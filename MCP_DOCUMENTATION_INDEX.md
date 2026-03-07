# MCP Server Integration - Complete Documentation

**Step-by-step guides for calling your CQRS Pattern MCP Server from Postman and Claude.**

---

## 📚 Documentation Files

| File | Purpose | Read Time |
|------|---------|-----------|
| **QUICK_START_MCP.md** | Get started in 5 minutes | 5 min ⚡ |
| **MCP_INTEGRATION_GUIDE.md** | Complete step-by-step guide | 15 min 📖 |
| **MCP_JSON_CONFIGURATION_GUIDE.md** | Detailed mcp.json reference | 10 min 🔧 |
| **CQRS_Pattern_MCP_Postman.postman_collection.json** | Pre-made Postman requests | 1 min 📤 |

---

## 🎯 Start Here

### **I want to test with Postman**
→ Go to [QUICK_START_MCP.md](QUICK_START_MCP.md) "Using with Postman" section (2 min)

### **I want to use Claude**
→ Go to [QUICK_START_MCP.md](QUICK_START_MCP.md) "Using with Claude" section (3 min)

### **I need detailed step-by-step instructions**
→ Read [MCP_INTEGRATION_GUIDE.md](MCP_INTEGRATION_GUIDE.md) (full guide)

### **I need to configure mcp.json for a specific scenario**
→ Check [MCP_JSON_CONFIGURATION_GUIDE.md](MCP_JSON_CONFIGURATION_GUIDE.md)

---

## 🚀 Quick Reference

### Starting the Server

**Option 1: Batch Script (Windows)**
```batch
setup-mcp.bat
# Choose option 1 to start API
```

**Option 2: PowerShell**
```powershell
.\setup-mcp.ps1 start-api
```

**Option 3: Manual**
```bash
cd CQRSPattern.Api
dotnet run
```

**Server runs at:** `http://localhost:5000/mcp`

### Testing with curl

```bash
# Get all employees
curl -X POST http://localhost:5000/mcp \
  -H "Content-Type: application/json" \
  -d '{
    "jsonrpc": "2.0",
    "id": 1,
    "method": "tools/call",
    "params": {
      "name": "get_all_employees",
      "arguments": {}
    }
  }'
```

### Using with Postman

1. Import: `CQRS_Pattern_MCP_Postman.postman_collection.json`
2. Click any request
3. Click **Send** ✓

### Using with Claude

1. Open Claude settings
2. Add MCP Server
3. Type: **HTTP**
4. URL: **`http://localhost:5000/mcp/request`**
5. Headers: **`Content-Type: application/json`**
6. Test connection ✓

---

## 🛠️ Setup Scripts

### Windows Batch (setup-mcp.bat)

```batch
setup-mcp.bat
```

**Features:**
- Start main API
- Start stdio server
- Test endpoints
- Show configuration
- Install/restore packages

### PowerShell (setup-mcp.ps1)

```powershell
# Show help
.\setup-mcp.ps1 help

# Start API
.\setup-mcp.ps1 start-api

# Start stdio server
.\setup-mcp.ps1 start-stdio

# Test MCP endpoints
.\setup-mcp.ps1 test

# Show configuration
.\setup-mcp.ps1 show-config

# Restore packages
.\setup-mcp.ps1 install
```

---

## 📦 Available MCP Tools

Your MCP server exposes 4 tools for employee management:

### 1. `get_all_employees`
**Get all employees**
```json
{
  "name": "get_all_employees",
  "arguments": {}
}
```

### 2. `add_employee`
**Add new employee**
```json
{
  "name": "add_employee",
  "arguments": {
    "firstName": "John",
    "lastName": "Doe",
    "gender": "Male",
    "birthDate": "1990-01-15",
    "hireDate": "2020-06-01"
  }
}
```

### 3. `update_employee`
**Replace entire employee record**
```json
{
  "name": "update_employee",
  "arguments": {
    "id": 1,
    "firstName": "Jonathan",
    "lastName": "Doe",
    "gender": "Male",
    "birthDate": "1990-01-15",
    "hireDate": "2020-06-01"
  }
}
```

### 4. `patch_employee`
**Update only specific fields (at least 1)**
```json
{
  "name": "patch_employee",
  "arguments": {
    "id": 1,
    "firstName": "Jonny"
  }
}
```

---

## 🔗 File Locations

```
CQRSPatternApi/
├── MCP_INTEGRATION_GUIDE.md           ← Start here for detailed guide
├── QUICK_START_MCP.md                 ← 5-minute quick start
├── MCP_JSON_CONFIGURATION_GUIDE.md    ← mcp.json reference
├── CQRS_Pattern_MCP_Postman.postman_collection.json  ← Import to Postman
├── setup-mcp.bat                      ← Windows setup script
├── setup-mcp.ps1                      ← PowerShell setup script
├── mcp.json                           ← Configuration file
│
├── CQRSPattern.Api/
│   ├── Program.cs
│   ├── Startup.Mcp.cs                 ← MCP server registration
│   └── Features/Mcp/Tools/
│       └── EmployeeTools.cs           ← Tool implementations
│
└── CQRSPatternMcpServer/
    ├── Program.cs                     ← stdio server startup
    └── ...
```

---

## 🆘 Troubleshooting

### Connection Refused
- Is the API running? `dotnet run` in CQRSPattern.Api/
- Check port: Should be `:5000`

### Tools Not Found
- Run `initialize` first
- Check API logs for startup errors

### Date Format Issues
- Use ISO 8601: `1990-01-15` ✓
- Not: `01/15/1990` ✗

### Patch Returns Error
- Provide at least one field beyond the ID

### Claude Not Finding Tools
- Verify mcp.json exists in workspace root
- Check HTTP server is accessible: `curl http://localhost:5000`
- Test in editor settings: "Test Connection"

---

## 📋 Checklist

### Local Development
- [ ] Run `dotnet run` in CQRSPattern.Api/ 
- [ ] Import Postman collection
- [ ] Test with "Get All Employees"
- [ ] Try adding a new employee
- [ ] Try patching an employee

### Claude Integration
- [ ] Server running on localhost:5000
- [ ] Open Claude settings
- [ ] Add MCP server (HTTP type)
- [ ] Use URL: `http://localhost:5000/mcp/request`
- [ ] Test connection
- [ ] Try a natural request: "Get all employees"

### Production
- [ ] Use HTTPS endpoints
- [ ] Add authentication headers
- [ ] Set appropriate timeouts
- [ ] Disable verbose logging
- [ ] Test with production credentials
- [ ] Monitor server logs

---

## 💡 Tips

✅ **Best Practices:**
- Start with Postman collection (easiest)
- Test each tool individually first
- Use stdio transport for CLI tools
- Use HTTP transport for web/Claude
- Always test connection before deploying

❌ **Common Mistakes:**
- Forgetting to start the API server
- Wrong date format in requests
- No space after "Bearer" in auth headers
- Setting timeouts too low
- Hardcoding secrets in mcp.json

---

## 📞 Support Resources

- **MCP Protocol:** https://modelcontextprotocol.io
- **Claude Help:** https://support.anthropic.com
- **Postman Docs:** https://learning.postman.com
- **Project README:** [README.md](README.md)
- **Architecture Docs:** [ARCHITECTURE.md](ARCHITECTURE.md)

---

## 🎓 Learning Path

**First Time?** Follow this order:

1. **Read:** [QUICK_START_MCP.md](QUICK_START_MCP.md)
2. **Do:** Start server with `setup-mcp.bat`
3. **Try:** Import & run Postman collection
4. **Learn:** Read [MCP_INTEGRATION_GUIDE.md](MCP_INTEGRATION_GUIDE.md)
5. **Configure:** Study [MCP_JSON_CONFIGURATION_GUIDE.md](MCP_JSON_CONFIGURATION_GUIDE.md)
6. **Deploy:** Deploy to production with HTTPS + auth

---

## 📝 File Descriptions

### QUICK_START_MCP.md
- 5-minute quick start guide
- Postman import instructions
- Claude setup in 4 steps
- Common curl examples
- Troubleshooting quick fixes

### MCP_INTEGRATION_GUIDE.md
- Complete architecture overview
- Step-by-step Postman instructions
- Detailed Claude integration guide
- All 4 tools explained with examples
- Advanced curl examples
- Full troubleshooting section

### MCP_JSON_CONFIGURATION_GUIDE.md
- What mcp.json does
- Configuration for each transport
- 5 real-world scenarios
- Property reference
- Environment variables
- Common issues & solutions

### CQRS_Pattern_MCP_Postman.postman_collection.json
- Pre-built Postman requests
- All 4 tools pre-configured
- Correct headers set
- Example parameters included
- 1 minute to import & use

### setup-mcp.bat
- Windows-friendly menu
- Start API server
- Start stdio server
- Test endpoints
- Show configuration
- Restore packages

### setup-mcp.ps1
- PowerShell version of batch script
- Same functionality
- Consistent parameter names
- Cross-platform compatible

---

## 🎯 Next Steps

**Pick your path:**

### → I want to use Postman
1. Download & install [Postman](https://postman.com)
2. Open `CQRS_Pattern_MCP_Postman.postman_collection.json`
3. Click "Import"
4. Start server: `dotnet run` or `setup-mcp.bat`
5. Click "Send" on any request ✓

### → I want to use Claude
1. Start server: `dotnet run` or `setup-mcp.bat`
2. Go to claude.ai (web or desktop)
3. Settings → Developer → MCP Config
4. Add server: `http://localhost:5000/mcp/request`
5. Chat: "Get all employees and..." ✓

### → I want to understand everything
1. Read: [QUICK_START_MCP.md](QUICK_START_MCP.md)
2. Read: [MCP_INTEGRATION_GUIDE.md](MCP_INTEGRATION_GUIDE.md)
3. Reference: [MCP_JSON_CONFIGURATION_GUIDE.md](MCP_JSON_CONFIGURATION_GUIDE.md)
4. Implement: Create custom mcp.json for your needs

---

**Questions, issues, or improvements?** Check the documentation or open an issue.

**Happy integrating! 🚀**

---

**Last Updated:** March 2026  
**Version:** 1.0  
**Status:** Complete & Production Ready
