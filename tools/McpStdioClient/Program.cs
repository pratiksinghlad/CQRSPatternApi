using System.Diagnostics;
using System.Text;

namespace McpStdioClient;

/// <summary>
/// Minimal JSON-RPC stdio client for calling an MCP server DLL.
/// Usage: dotnet run -- "<mcp-server-dll-path>" "<request-json-path>"
/// </summary>
internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Usage: <mcp-server-dll-path> <request-json-path>");
            return 2;
        }

        var dllPath = args[0];
        var requestJsonPath = args[1];

        if (!File.Exists(dllPath))
        {
            Console.Error.WriteLine($"DLL not found: {dllPath}");
            return 3;
        }

        if (!File.Exists(requestJsonPath))
        {
            Console.Error.WriteLine($"Request JSON not found: {requestJsonPath}");
            return 4;
        }

        var requestJson = await File.ReadAllTextAsync(requestJsonPath).ConfigureAwait(false);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"\"{dllPath}\"",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        bool noHeader = args.Length > 2 && args[2] == "--no-header";

        using var process = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start process");
        
        // Forward server stderr to console
        _ = Task.Run(async () =>
        {
            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    var line = await process.StandardError.ReadLineAsync(cts.Token).ConfigureAwait(false);
                    if (line is null) break;
                    await Console.Error.WriteLineAsync($"[SERVER] {line}").ConfigureAwait(false);
                }
            }
            catch { /* ignore */ }
        }, cts.Token);

        try
        {
            // Send JSON-RPC request with Content-Length framing
            // Normalize body line endings to LF to avoid CR/LF differences between platforms
            var normalizedBody = requestJson.Replace("\r\n", "\n").Replace("\r", "\n");
            var bodyBytes = Encoding.UTF8.GetBytes(normalizedBody);
            if (!noHeader)
            {
                // Use LF-LF as header terminator to match transports that look for \n\n
                var header = $"Content-Length: {bodyBytes.Length}\n\n";
                var headerBytes = Encoding.ASCII.GetBytes(header);

                // Write header and body together in a single write to avoid the server reading a
                // partial header or body which can confuse JSON framing on some platforms.
                var combined = new byte[headerBytes.Length + bodyBytes.Length];
                Buffer.BlockCopy(headerBytes, 0, combined, 0, headerBytes.Length);
                Buffer.BlockCopy(bodyBytes, 0, combined, headerBytes.Length, bodyBytes.Length);

                // DIAGNOSTIC: print hex dump of what we're about to write
                Console.Error.WriteLine("[CLIENT-HEX] Sending (header+body) bytes:");
                Console.Error.WriteLine(BitConverter.ToString(combined).Replace("-", " "));

                await process.StandardInput.BaseStream.WriteAsync(combined, 0, combined.Length, cts.Token).ConfigureAwait(false);
            }
            else
            {
                // Send only the JSON body (no Content-Length header) — used for diagnostics
                Console.Error.WriteLine("[CLIENT-HEX] Sending (body-only) bytes:");
                Console.Error.WriteLine(BitConverter.ToString(bodyBytes).Replace("-", " "));
                await process.StandardInput.BaseStream.WriteAsync(bodyBytes, 0, bodyBytes.Length, cts.Token).ConfigureAwait(false);
            }
            await process.StandardInput.BaseStream.FlushAsync(cts.Token).ConfigureAwait(false);

            Console.Error.WriteLine($"[CLIENT] Sent request ({bodyBytes.Length} bytes)");

            // Read response headers
            var responseHeaders = await ReadHeadersAsync(process.StandardOutput.BaseStream, cts.Token).ConfigureAwait(false);
            var contentLength = ParseContentLength(responseHeaders);
            
            if (contentLength <= 0)
            {
                Console.Error.WriteLine("[CLIENT] Invalid or missing Content-Length in response");
                return 5;
            }

            Console.Error.WriteLine($"[CLIENT] Reading response body ({contentLength} bytes)");

            // Read response body
            var responseBody = await ReadExactAsync(process.StandardOutput.BaseStream, contentLength, cts.Token).ConfigureAwait(false);
            var responseString = Encoding.UTF8.GetString(responseBody);
            
            Console.WriteLine(responseString);
            return 0;
        }
        finally
        {
            try { process.Kill(); } catch { /* ignore */ }
        }
    }

    private static async Task<string> ReadHeadersAsync(Stream stream, CancellationToken ct)
    {
        using var ms = new MemoryStream();
        var buffer = new byte[1];
        var sequence = 0; // track \r\n\r\n
        
        while (!ct.IsCancellationRequested)
        {
            var read = await stream.ReadAsync(buffer.AsMemory(0, 1), ct).ConfigureAwait(false);
            if (read == 0) break;
            
            ms.WriteByte(buffer[0]);
            
            if (buffer[0] == '\r') sequence = (sequence % 2 == 0) ? sequence + 1 : 1;
            else if (buffer[0] == '\n') sequence = (sequence % 2 == 1) ? sequence + 1 : 0;
            else sequence = 0;
            
            if (sequence == 4) break; // found \r\n\r\n
        }
        
        return Encoding.ASCII.GetString(ms.ToArray());
    }

    private static int ParseContentLength(string headerText)
    {
        using var sr = new StringReader(headerText);
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            var idx = line.IndexOf("Content-Length:", StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                var value = line[(idx + "Content-Length:".Length)..].Trim();
                if (int.TryParse(value, out var len)) return len;
            }
        }
        return -1;
    }

    private static async Task<byte[]> ReadExactAsync(Stream stream, int length, CancellationToken ct)
    {
        var buffer = new byte[length];
        var offset = 0;
        while (offset < length)
        {
            var read = await stream.ReadAsync(buffer.AsMemory(offset, length - offset), ct).ConfigureAwait(false);
            if (read == 0) throw new EndOfStreamException("Stream closed before reading full body");
            offset += read;
        }
        return buffer;
    }
}
