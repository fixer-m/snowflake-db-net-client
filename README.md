[![NuGet](https://img.shields.io/badge/nuget-v0.3.0-green.svg)](https://www.nuget.org/packages/Snowflake.Client/) 
[![](https://img.shields.io/nuget/dt/Snowflake.Client.svg)](https://www.nuget.org/packages/Snowflake.Client/) 
[![Targets](https://img.shields.io/badge/.NET%20Standard-2.0-green.svg)](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) 
[![License](https://img.shields.io/badge/License-Apache%202.0-green.svg)](https://opensource.org/licenses/Apache-2.0)

## Snowflake.Client
Unofficial .NET client for [Snowflake](https://www.snowflake.com) REST API.  
Execute SQL queries in Snowflake and get mapped response back.  
Read my [blog post](https://medium.com/@fixer_m/better-net-client-for-snowflake-db-ecb48c48c872) about it. 

### Main Features
- Basic authentication
- Execute SQL queries with parameters
- Map response data to your models

### Basic Usage
```csharp
// Creates new client
var snowflakeClient = new SnowflakeClient("user", "password", "account", "region");

// Executes query and maps response data to "Employee" class
var employees = await snowflakeClient.QueryAsync<Employee>("SELECT * FROM MASTER.PUBLIC.EMPLOYEES;");

// Executes query and returns raw response from Snowflake (rows, columns and query information)
var queryRawResponse = await snowflakeClient.QueryRawResponseAsync("SELECT * FROM MASTER.PUBLIC.EMPLOYEES;");

// Executes query and returns value of first cell as string result
string useRoleResult = await snowflakeClient.ExecuteScalarAsync("USE ROLE ACCOUNTADMIN;");

// Executes query and returns affected rows count
int affectedRows = await snowflakeClient.ExecuteAsync("INSERT INTO EMPLOYEES Title VALUES (?);", "Dev");

// Parameters binding options:
var employees1 = await snowflakeClient.QueryAsync<Employee>("SELECT * FROM EMPLOYEES WHERE TITLE = ?", "Programmer");
var employees2 = await snowflakeClient.QueryAsync<Employee>("SELECT * FROM EMPLOYEES WHERE ID IN (?,?)", new int[] { 1, 2 });
var employees3 = await snowflakeClient.QueryAsync<Employee>("SELECT * FROM EMPLOYEES WHERE TITLE = :Title", new Employee() { Title = "Programmer" });
var employees4 = await snowflakeClient.QueryAsync<Employee>("SELECT * FROM EMPLOYEES WHERE TITLE = :Title", new { Title = "Programmer" });
```

### Comparison with Snowflake.Data 
Official [Snowflake.Data](https://github.com/snowflakedb/snowflake-connector-net) connector implements ADO.NET interfaces (IDbConnection, IDataReader etc), so you have to work with it as with usual database on a low level (however under the hood it actually uses Snowflake REST API). In contrast this library is designed as REST API client (or wrapper) with straightforward and clean API. [Read more](https://medium.com/@fixer_m/better-net-client-for-snowflake-db-ecb48c48c872) about it.

Improvements in Snowflake.Client vs Snowflake.Data: 
- Performance: Re-uses Snowflake session, i.e. less roundtrips to SF
- Performance: Doesn't have additional intermediate mapping from SF to DB types 
- Better API: Clean and simple syntax vs verbose ADO.NET syntax
- Third party dependencies: 0 vs 4

Added features in Snowflake.Client vs Snowflake.Data:
- Map response data to entities
- Supports describeOnly flag
- Has option to return raw data from Snowflake response
- Exposes Snowflake session info
- New SQL parameter binding API with a few options (inspired by Dapper)

Missing features in Snowflake.Client vs Snowflake.Data:
- Chunks downloader (to download big amount of data)
- Auto-renew session 
- OKTA Authentication

### Installation
Add nuget package [Snowflake.Client](https://www.nuget.org/packages/Snowflake.Client) to your project:  
```{r, engine='bash', code_block_name}
PM> Install-Package Snowflake.Client
```

### Road Map 
- [Done] Async API 
- [In Progress] Unit tests
- Integration tests
- Chunks downloader (for big amount of data)
- Auto-renew session 
- OKTA Authentication
- Query cancellation
- Mapper documentation
