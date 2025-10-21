using System;
using System.Net.Http;
using System.Threading.Tasks;

var apiUrl = Environment.GetEnvironmentVariable("CQRS_API_URL") ?? "http://localhost:5001/";
using var client = new HttpClient { BaseAddress = new Uri(apiUrl) };
try
{
    var res = await client.GetAsync("/api/employees");
    res.EnsureSuccessStatusCode();
    var body = await res.Content.ReadAsStringAsync();
    Console.WriteLine(body);
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
    Environment.Exit(1);
}
