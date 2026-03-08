# ================================================================
# CQRS Pattern MCP Server - Setup Script (PowerShell)
# ================================================================
# This script helps with:
# 1. Starting the main API (HTTP transport)
# 2. Starting the MCP stdio server
# 3. Testing MCP endpoints
# ================================================================

param(
    [Parameter(Mandatory = $false)]
    [ValidateSet("start-api", "start-stdio", "test", "show-config", "install", "help")]
    [string]$Command = "help"
)

$ScriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$ApiPath = Join-Path $ScriptPath "CQRSPattern.Api"
$McpServerPath = Join-Path $ScriptPath "CQRSPatternMcpServer"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "CQRS Pattern MCP Server Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

function Show-Menu {
    Write-Host "Available commands:" -ForegroundColor Green
    Write-Host ""
    Write-Host "  .\setup-mcp.ps1 start-api      - Start main API (localhost:5001)" -ForegroundColor Yellow
    Write-Host "  .\setup-mcp.ps1 start-stdio    - Start MCP stdio server" -ForegroundColor Yellow
    Write-Host "  .\setup-mcp.ps1 test           - Test MCP endpoints with curl" -ForegroundColor Yellow
    Write-Host "  .\setup-mcp.ps1 show-config    - Display mcp.json configuration" -ForegroundColor Yellow
    Write-Host "  .\setup-mcp.ps1 install        - Restore NuGet packages" -ForegroundColor Yellow
    Write-Host "  .\setup-mcp.ps1 help           - Show this help message" -ForegroundColor Yellow
    Write-Host ""
}

function Start-MainApi {
    Write-Host ""
    Write-Host "Starting CQRSPattern.Api on localhost:5001..." -ForegroundColor Green
    Write-Host ""
    Write-Host "MCP Endpoint: http://localhost:5001/mcp" -ForegroundColor Cyan
    Write-Host "Press Ctrl+C to stop." -ForegroundColor Gray
    Write-Host ""
    
    Set-Location $ApiPath
    dotnet run
}

function Start-StdioServer {
    Write-Host ""
    Write-Host "Starting MCP stdio Server..." -ForegroundColor Green
    Write-Host ""
    Write-Host "This starts the standalone MCP server with stdio transport." -ForegroundColor Cyan
    Write-Host "Use this for Claude integration without the main API." -ForegroundColor Gray
    Write-Host "Press Ctrl+C to stop." -ForegroundColor Gray
    Write-Host ""
    
    Set-Location $McpServerPath
    dotnet run
}

function Test-McpServer {
    Write-Host ""
    Write-Host "Testing MCP Server..." -ForegroundColor Green
    Write-Host ""
    
    # Test connection
    Write-Host "Testing connection to http://localhost:5001/mcp..." -ForegroundColor Yellow
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5001" -Method GET -TimeoutSec 2 -ErrorAction SilentlyContinue
        Write-Host "✓ Connected!" -ForegroundColor Green
    }
    catch {
        Write-Host "✗ Cannot connect to http://localhost:5001" -ForegroundColor Red
        Write-Host ""
        Write-Host "Please start the API first:" -ForegroundColor Yellow
        Write-Host "  .\setup-mcp.ps1 start-api" -ForegroundColor Cyan
        Write-Host ""
        return
    }
    
    Write-Host ""
    
    # Test 1: Initialize
    Write-Host "[TEST 1] Initializing MCP protocol..." -ForegroundColor Yellow
    $initBody = @{
        jsonrpc = "2.0"
        id      = 1
        method  = "initialize"
        params  = @{
            protocolVersion = "2024-11-05"
            capabilities    = @{}
            clientInfo      = @{
                name    = "PowerShell"
                version = "1.0"
            }
        }
    } | ConvertTo-Json

    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5001/mcp" `
            -Method POST `
            -Headers @{"Content-Type" = "application/json"} `
            -Body $initBody `
            -TimeoutSec 5

        if ($response.Content -match "protocolVersion") {
            Write-Host "✓ Initialize successful" -ForegroundColor Green
        }
        else {
            Write-Host "✗ Initialize failed" -ForegroundColor Red
        }
    }
    catch {
        Write-Host "✗ Initialize failed: $_" -ForegroundColor Red
    }
    
    Write-Host ""
    
    # Test 2: List tools
    Write-Host "[TEST 2] Listing available tools..." -ForegroundColor Yellow
    $listBody = @{
        jsonrpc = "2.0"
        id      = 2
        method  = "tools/list"
        params  = @{}
    } | ConvertTo-Json

    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5001/mcp" `
            -Method POST `
            -Headers @{"Content-Type" = "application/json"} `
            -Body $listBody `
            -TimeoutSec 5

        if ($response.Content -match "get_all_employees") {
            Write-Host "✓ Tools listed successfully" -ForegroundColor Green
        }
        else {
            Write-Host "✗ Tools listing failed" -ForegroundColor Red
        }
    }
    catch {
        Write-Host "✗ Tools listing failed: $_" -ForegroundColor Red
    }
    
    Write-Host ""
    
    # Test 3: Call tool
    Write-Host "[TEST 3] Calling get_all_employees tool..." -ForegroundColor Yellow
    $callBody = @{
        jsonrpc = "2.0"
        id      = 3
        method  = "tools/call"
        params  = @{
            name      = "get_all_employees"
            arguments = @{}
        }
    } | ConvertTo-Json

    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5001/mcp" `
            -Method POST `
            -Headers @{"Content-Type" = "application/json"} `
            -Body $callBody `
            -TimeoutSec 5

        if ($response.Content -match '"result"') {
            Write-Host "✓ Tool call successful" -ForegroundColor Green
        }
        else {
            Write-Host "✗ Tool call failed" -ForegroundColor Red
        }
    }
    catch {
        Write-Host "✗ Tool call failed: $_" -ForegroundColor Red
    }
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Test Summary" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "If all tests passed, your MCP server is working!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "  1. Import CQRS_Pattern_MCP_Postman.postman_collection.json into Postman" -ForegroundColor Gray
    Write-Host "  2. Or configure mcp.json for Claude integration" -ForegroundColor Gray
    Write-Host ""
}

function Show-Config {
    Write-Host ""
    Write-Host "Current mcp.json Configuration" -ForegroundColor Green
    Write-Host ""
    
    $configPath = Join-Path $ScriptPath "mcp.json"
    if (Test-Path $configPath) {
        Get-Content $configPath | Write-Host -ForegroundColor Gray
    }
    else {
        Write-Host "ERROR: mcp.json not found at $configPath" -ForegroundColor Red
    }
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Configuration Details" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "1. cqrspattern-http-direct:" -ForegroundColor Yellow
    Write-Host "   Type: HTTP" -ForegroundColor Gray
    Write-Host "   URL: http://localhost:5001/mcp" -ForegroundColor Gray
    Write-Host "   Use: Postman, web clients, Claude with main API running" -ForegroundColor Gray
    Write-Host ""
    Write-Host "2. cqrspattern-stdio:" -ForegroundColor Yellow
    Write-Host "   Type: stdio" -ForegroundColor Gray
    Write-Host "   Command: dotnet run --project CQRSPatternMcpServer" -ForegroundColor Gray
    Write-Host "   Use: Claude Code, CLI tools" -ForegroundColor Gray
    Write-Host ""
}

function Install-Dependencies {
    Write-Host ""
    Write-Host "Installing/updating dependencies..." -ForegroundColor Green
    Write-Host ""
    
    Write-Host "Restoring API packages..." -ForegroundColor Yellow
    Set-Location $ApiPath
    dotnet restore
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Failed to restore API packages" -ForegroundColor Red
        return
    }
    
    Write-Host "Restoring MCP Server packages..." -ForegroundColor Yellow
    Set-Location $McpServerPath
    dotnet restore
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Failed to restore MCP Server packages" -ForegroundColor Red
        return
    }
    
    Write-Host ""
    Write-Host "✓ All packages restored successfully!" -ForegroundColor Green
    Write-Host ""
}

# Route commands
switch ($Command) {
    "start-api" { Start-MainApi }
    "start-stdio" { Start-StdioServer }
    "test" { Test-McpServer }
    "show-config" { Show-Config }
    "install" { Install-Dependencies }
    "help" { Show-Menu }
    default { Show-Menu }
}

Write-Host ""
