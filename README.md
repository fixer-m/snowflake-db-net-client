[![NuGet](https://img.shields.io/badge/nuget-v0.2.2-green.svg)](https://www.nuget.org/packages/Snowflake.Client/) 
[![](https://img.shields.io/nuget/dt/Snowflake.Client.svg)](https://www.nuget.org/packages/Snowflake.Client/) 
[![Targets](https://img.shields.io/badge/.NET%20Standard-2.0-green.svg)](https://docs.microsoft.com/ru-ru/dotnet/standard/net-standard) 
[![License](https://img.shields.io/badge/License-Apache%202.0-green.svg)](https://opensource.org/licenses/Apache-2.0)

## Snowflake.Client
Unofficial .NET client for [Snowflake](https://www.snowflake.com) REST API.  
Provides straightforward and efficient way to execute SQL queries in Snowflake and automatically maps response to your models. 

### Main Features
- Basic authentication
- Execute SQL queries with parameters
- Map response data to your models

### Basic Usage
```csharp
// Creates new client and initializes new session 
var snowflakeClient = new SnowflakeClient("user", "password", "account", "region");

// Executes query and maps response data to your class
var employees = snowflakeClient.Query<Employee>("SELECT * FROM MASTER.PUBLIC.EMPLOYEES");

// Executes query and returns raw data from response (rows and columns)
var employeesRawData = snowflakeClient.QueryRaw("SELECT * FROM MASTER.PUBLIC.EMPLOYEES");

// Executes scalar query and returns numeric result
long employeesCount  = snowflakeClient.ExecuteScalar("SELECT COUNT(*) FROM MASTER.PUBLIC.EMPLOYEES");

// Executes non-query request and returns string result
string useRoleResult = snowflakeClient.ExecuteNonQuery("USE MASTER.PUBLIC;");

// Parameters binding options:
var employeesParam_1 = snowflakeClient.Query<Employee>("SELECT * FROM EMPLOYEES WHERE TITLE = ?", "Programmer");
var employeesParam_2 = snowflakeClient.Query<Employee>("SELECT * FROM EMPLOYEES WHERE ID IN (?,?)", new int[] { 1, 2 });
var employeesParam_3 = snowflakeClient.Query<Employee>("SELECT * FROM EMPLOYEES WHERE TITLE = :Title", new Employee() { Title = "Programmer" });
var employeesParam_4 = snowflakeClient.Query<Employee>("SELECT * FROM EMPLOYEES WHERE TITLE = :Title", new { Title = "Programmer" });
```

### Comparison with Snowflake.Data 
Official [Snowflake.Data](https://github.com/snowflakedb/snowflake-connector-net) connector implements ADO.NET interfaces (IDbConnection, IDataReader etc), so you have to work with it as with usual database. Under the hood it actually uses Snowflake REST API. In contrast this library is designed as native client for Snowflake REST API. This approach doesn't have restrictions and limitations that official library has. As well as it gives much more freedom to develop new features. 

Improvements in Snowflake.Client vs Snowflake.Data: 
- Performance: Re-uses Snowflake session, i.e. less roundtrips to SF
- Performance: Faster json (de)serialize with System.Text.Json (vs Newtonsoft.JSON)
- Performance: Doesn't have additional intermediate mapping from SF to DB types 
- Better API: Clean and simple syntax vs verbose ADO.NET syntax
- Less 3rd party dependencies, uses only MS packages 

New features in Snowflake.Client:
- Map response data to entities
- Supports describeOnly flag
- Has option to return raw data from Snowflake response
- Exposes Snowflake session information
- New SQL parameter binding with a few options (inspired by Dapper)

Currently missing features in Snowflake.Client:
- Chunks downloader (for big amount of data)
- Auto-renew session 
- Async API
- OKTA Authentication

### Installation
Add nuget package [Snowflake.Client](https://www.nuget.org/packages/Snowflake.Client) to your project:  
```{r, engine='bash', code_block_name}
PM> Install-Package Snowflake.Client
```

### Road Map 
- Chunks downloader (for big amount of data)
- Auto-renew session 
- Async API
- OKTA Authentication
- Unit tests
- Query cancellation
- Mapper documentation
