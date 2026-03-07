#!/bin/bash

################################################################################
# CQRS Pattern MCP Server - Setup Script (macOS/Linux)
#
# This script helps with:
# 1. Starting the main API (HTTP transport)
# 2. Starting the MCP stdio server
# 3. Testing MCP endpoints
################################################################################

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
API_PATH="$SCRIPT_DIR/CQRSPattern.Api"
MCP_SERVER_PATH="$SCRIPT_DIR/CQRSPatternMcpServer"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Helper functions
print_header() {
    echo ""
    echo -e "${BLUE}================================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}================================================${NC}"
    echo ""
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_info() {
    echo -e "${YELLOW}ℹ $1${NC}"
}

show_menu() {
    echo ""
    echo -e "${GREEN}Available commands:${NC}"
    echo ""
    echo -e "  ${YELLOW}./setup-mcp.sh start-api${NC}      - Start main API (localhost:5000)"
    echo -e "  ${YELLOW}./setup-mcp.sh start-stdio${NC}    - Start MCP stdio server"
    echo -e "  ${YELLOW}./setup-mcp.sh test${NC}           - Test MCP endpoints"
    echo -e "  ${YELLOW}./setup-mcp.sh show-config${NC}    - Display mcp.json configuration"
    echo -e "  ${YELLOW}./setup-mcp.sh install${NC}        - Restore NuGet packages"
    echo -e "  ${YELLOW}./setup-mcp.sh help${NC}           - Show this help message"
    echo ""
}

start_api() {
    echo ""
    print_info "Starting CQRSPattern.Api on localhost:5000..."
    echo ""
    echo -e "${BLUE}MCP Endpoint: http://localhost:5000/mcp${NC}"
    print_info "Press Ctrl+C to stop."
    echo ""
    
    cd "$API_PATH"
    dotnet run
}

start_stdio() {
    echo ""
    print_info "Starting MCP stdio Server..."
    echo ""
    echo -e "${BLUE}This starts the standalone MCP server with stdio transport.${NC}"
    print_info "Use this for Claude integration without the main API."
    print_info "Press Ctrl+C to stop."
    echo ""
    
    cd "$MCP_SERVER_PATH"
    dotnet run
}

test_server() {
    echo ""
    print_header "Testing MCP Server"
    
    # Test connection
    print_info "Testing connection to http://localhost:5000/mcp..."
    
    if ! command -v curl &> /dev/null; then
        print_error "curl is not installed. Installing curl..."
        if command -v brew &> /dev/null; then
            brew install curl
        else
            print_error "Please install curl manually: https://curl.se"
            return 1
        fi
    fi
    
    if ! curl -s http://localhost:5000 > /dev/null 2>&1; then
        print_error "Cannot connect to http://localhost:5000"
        echo ""
        print_info "Please start the API first:"
        echo -e "${BLUE}  ./setup-mcp.sh start-api${NC}"
        echo ""
        return 1
    fi
    
    print_success "Connected!"
    echo ""
    
    # Test 1: Initialize
    print_info "[TEST 1] Initializing MCP protocol..."
    INIT_BODY='{
        "jsonrpc": "2.0",
        "id": 1,
        "method": "initialize",
        "params": {
            "protocolVersion": "2024-11-05",
            "capabilities": {},
            "clientInfo": {
                "name": "bash-test",
                "version": "1.0"
            }
        }
    }'
    
    RESPONSE=$(curl -s -X POST http://localhost:5000/mcp \
        -H "Content-Type: application/json" \
        -d "$INIT_BODY")
    
    if echo "$RESPONSE" | grep -q "protocolVersion"; then
        print_success "Initialize successful"
    else
        print_error "Initialize failed"
    fi
    
    echo ""
    
    # Test 2: List tools
    print_info "[TEST 2] Listing available tools..."
    LIST_BODY='{
        "jsonrpc": "2.0",
        "id": 2,
        "method": "tools/list",
        "params": {}
    }'
    
    RESPONSE=$(curl -s -X POST http://localhost:5000/mcp \
        -H "Content-Type: application/json" \
        -d "$LIST_BODY")
    
    if echo "$RESPONSE" | grep -q "get_all_employees"; then
        print_success "Tools listed successfully"
    else
        print_error "Tools listing failed"
    fi
    
    echo ""
    
    # Test 3: Call tool
    print_info "[TEST 3] Calling get_all_employees tool..."
    CALL_BODY='{
        "jsonrpc": "2.0",
        "id": 3,
        "method": "tools/call",
        "params": {
            "name": "get_all_employees",
            "arguments": {}
        }
    }'
    
    RESPONSE=$(curl -s -X POST http://localhost:5000/mcp \
        -H "Content-Type: application/json" \
        -d "$CALL_BODY")
    
    if echo "$RESPONSE" | grep -q '"result"'; then
        print_success "Tool call successful"
    else
        print_error "Tool call failed"
    fi
    
    echo ""
    print_header "Test Summary"
    
    echo -e "${GREEN}If all tests passed, your MCP server is working!${NC}"
    echo ""
    print_info "Next steps:"
    echo "  1. Import CQRS_Pattern_MCP_Postman.postman_collection.json into Postman"
    echo "  2. Or configure mcp.json for Claude integration"
    echo ""
}

show_config() {
    echo ""
    print_header "Current mcp.json Configuration"
    
    CONFIG_FILE="$SCRIPT_DIR/mcp.json"
    if [ -f "$CONFIG_FILE" ]; then
        cat "$CONFIG_FILE"
    else
        print_error "mcp.json not found at $CONFIG_FILE"
        return 1
    fi
    
    echo ""
    print_header "Configuration Details"
    
    echo -e "${YELLOW}1. cqrspattern-http-direct:${NC}"
    echo "   Type: HTTP"
    echo "   URL: http://localhost:5000/mcp/request"
    echo "   Use: Postman, web clients, Claude with main API running"
    echo ""
    
    echo -e "${YELLOW}2. cqrspattern-stdio:${NC}"
    echo "   Type: stdio"
    echo "   Command: dotnet run --project CQRSPatternMcpServer"
    echo "   Use: Claude Code, CLI tools"
    echo ""
}

install_dependencies() {
    echo ""
    print_info "Installing/updating dependencies..."
    echo ""
    
    print_info "Checking for .NET SDK..."
    if ! command -v dotnet &> /dev/null; then
        print_error ".NET SDK is not installed"
        echo ""
        print_info "Install from: https://dotnet.microsoft.com/download"
        return 1
    fi
    
    DOTNET_VERSION=$(dotnet --version)
    print_success "Found .NET SDK: $DOTNET_VERSION"
    echo ""
    
    print_info "Restoring API packages..."
    cd "$API_PATH"
    if dotnet restore; then
        print_success "API packages restored"
    else
        print_error "Failed to restore API packages"
        return 1
    fi
    
    echo ""
    print_info "Restoring MCP Server packages..."
    cd "$MCP_SERVER_PATH"
    if dotnet restore; then
        print_success "MCP Server packages restored"
    else
        print_error "Failed to restore MCP Server packages"
        return 1
    fi
    
    echo ""
    print_success "All packages restored successfully!"
    echo ""
}

# Main script
if [ $# -eq 0 ]; then
    print_header "CQRS Pattern MCP Server Setup"
    show_menu
    exit 0
fi

case "$1" in
    start-api)
        start_api
        ;;
    start-stdio)
        start_stdio
        ;;
    test)
        test_server
        ;;
    show-config)
        show_config
        ;;
    install)
        install_dependencies
        ;;
    help)
        print_header "CQRS Pattern MCP Server Setup"
        show_menu
        ;;
    *)
        print_error "Unknown command: $1"
        echo ""
        show_menu
        exit 1
        ;;
esac

exit $?
