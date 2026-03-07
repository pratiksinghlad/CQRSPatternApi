@echo off
REM ================================================================
REM CQRS Pattern MCP Server - Quick Setup & Test Script
REM ================================================================
REM This script helps you:
REM 1. Start the main API with HTTP transport
REM 2. Start the MCP stdio server
REM 3. Test MCP endpoints with curl
REM ================================================================

setlocal enabledelayedexpansion

color 0A
cls

echo.
echo ================================================================
echo   CQRS Pattern MCP Server - Setup & Test
echo ================================================================
echo.

:menu
echo.
echo Choose an option:
echo.
echo   1 - Start Main API (HTTP Transport on localhost:5001)
echo   2 - Start MCP stdio Server (for Claude integration)
echo   3 - Test MCP with curl (requires API running)
echo   4 - Show mcp.json configuration
echo   5 - Open documentation (MCP_INTEGRATION_GUIDE.md)
echo   6 - Install/Update dependencies
echo   7 - Exit
echo.

set /p choice="Enter your choice (1-7): "

if "%choice%"=="1" goto start_api
if "%choice%"=="2" goto start_stdio
if "%choice%"=="3" goto test_curl
if "%choice%"=="4" goto show_config
if "%choice%"=="5" goto open_docs
if "%choice%"=="6" goto dependencies
if "%choice%"=="7" goto exit

echo.
echo Invalid choice. Please try again.
goto menu

REM ================================================================
REM Option 1: Start Main API
REM ================================================================
:start_api
cls
echo.
echo Starting CQRSPattern.Api on localhost:5001...
echo.
echo MCP Endpoint: http://localhost:5001/mcp
echo.
echo Press Ctrl+C to stop.
echo.
cd /d "%~dp0CQRSPattern.Api"
dotnet run
pause
goto menu

REM ================================================================
REM Option 2: Start stdio Server
REM ================================================================
:start_stdio
cls
echo.
echo Starting MCP stdio Server...
echo.
echo This starts the standalone MCP server with stdio transport.
echo Use this for Claude integration without the main API.
echo.
echo Press Ctrl+C to stop.
echo.
cd /d "%~dp0CQRSPatternMcpServer"
dotnet run
pause
goto menu

REM ================================================================
REM Option 3: Test with curl
REM ================================================================
:test_curl
cls
echo.
echo ================================================================
echo   Testing MCP Server
echo ================================================================
echo.

REM Check if API is running
echo Testing connection to http://localhost:5001/mcp...
curl -s http://localhost:5001 >nul 2>&1
if errorlevel 1 (
    echo.
    echo ERROR: Cannot connect to http://localhost:5001
    echo.
    echo Please start the API first (option 1)
    echo.
    pause
    goto menu
)

echo Connected! Now testing MCP endpoints...
echo.

REM Test 1: Initialize
echo [TEST 1] Initializing MCP protocol...
curl -X POST http://localhost:5001/mcp ^
  -H "Content-Type: application/json" ^
  -d "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"initialize\",\"params\":{\"protocolVersion\":\"2024-11-05\",\"capabilities\":{},\"clientInfo\":{\"name\":\"test\",\"version\":\"1.0\"}}}" 2>nul | findstr /c:"protocolVersion" >nul
if %errorlevel%==0 (
    echo ✓ Initialize successful
) else (
    echo ✗ Initialize failed
)
echo.

REM Test 2: List tools
echo [TEST 2] Listing available tools...
curl -X POST http://localhost:5001/mcp ^
  -H "Content-Type: application/json" ^
  -d "{\"jsonrpc\":\"2.0\",\"id\":2,\"method\":\"tools/list\",\"params\":{}}" 2>nul | findstr /c:"get_all_employees" >nul
if %errorlevel%==0 (
    echo ✓ Tools listed successfully
) else (
    echo ✗ Tools listing failed
)
echo.

REM Test 3: Get all employees
echo [TEST 3] Calling get_all_employees tool...
curl -X POST http://localhost:5001/mcp ^
  -H "Content-Type: application/json" ^
  -d "{\"jsonrpc\":\"2.0\",\"id\":3,\"method\":\"tools/call\",\"params\":{\"name\":\"get_all_employees\",\"arguments\":{}}}" 2>nul | findstr /c:"\"result\"" >nul
if %errorlevel%==0 (
    echo ✓ Tool call successful
) else (
    echo ✗ Tool call failed
)
echo.

echo ================================================================
echo   Test Results Summary
echo ================================================================
echo.
echo If all tests passed, your MCP server is working correctly!
echo.
echo Next steps:
echo   1. Write down the curl commands above for your tests
echo   2. Use them in Postman (paste request URL and body)
echo   3. Or configure mcp.json for Claude integration
echo.

pause
goto menu

REM ================================================================
REM Option 4: Show mcp.json
REM ================================================================
:show_config
cls
echo.
echo ================================================================
echo   Current mcp.json Configuration
echo ================================================================
echo.

if exist "%~dp0mcp.json" (
    type "%~dp0mcp.json"
) else (
    echo ERROR: mcp.json not found at %~dp0
)

echo.
echo ================================================================
echo   Configuration Explanation
echo ================================================================
echo.
echo 1. cqrspattern-http-direct:
echo    - Type: HTTP (for Postman, web clients)
echo    - URL: http://localhost:5001/mcp
echo    - Use this for Claude web/desktop with main API running
echo.
echo 2. cqrspattern-stdio:
echo    - Type: stdio (for CLI, Claude Code)
echo    - Command: dotnet run --project CQRSPatternMcpServer
echo    - Use this for Claude integration without main API
echo.
echo ================================================================
echo.

pause
goto menu

REM ================================================================
REM Option 5: Open Documentation
REM ================================================================
:open_docs
if exist "%~dp0MCP_INTEGRATION_GUIDE.md" (
    start notepad.exe "%~dp0MCP_INTEGRATION_GUIDE.md"
) else (
    echo ERROR: MCP_INTEGRATION_GUIDE.md not found
    pause
)
goto menu

REM ================================================================
REM Option 6: Install/Update Dependencies
REM ================================================================
:dependencies
cls
echo.
echo Installing/updating dependencies...
echo.

cd /d "%~dp0"

echo Restoring main API packages...
cd "%~dp0CQRSPattern.Api"
dotnet restore
if errorlevel 1 (
    echo ERROR: Failed to restore API packages
    pause
    goto menu
)

echo Restoring MCP Server packages...
cd "%~dp0CQRSPatternMcpServer"
dotnet restore
if errorlevel 1 (
    echo ERROR: Failed to restore MCP Server packages
    pause
    goto menu
)

echo.
echo ✓ All packages restored successfully!
echo.

pause
goto menu

REM ================================================================
REM Exit
REM ================================================================
:exit
cls
echo.
echo Thank you for using CQRS Pattern MCP Server!
echo.
echo Documentation: MCP_INTEGRATION_GUIDE.md
echo Repository: https://github.com/yourusername/CQRSPatternApi
echo.
timeout /t 2 /nobreak
exit /b 0
