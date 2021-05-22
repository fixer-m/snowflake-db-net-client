[![NuGet](https://img.shields.io/badge/nuget-v0.3.5-green.svg)](https://www.nuget.org/packages/Snowflake.Client/) 
[![](https://img.shields.io/nuget/dt/Snowflake.Client.svg)](https://www.nuget.org/packages/Snowflake.Client/) 
[![Targets](https://img.shields.io/badge/.NET%20Standard-2.0-green.svg)](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) 
[![License](https://img.shields.io/badge/License-Apache%202.0-green.svg)](https://opensource.org/licenses/Apache-2.0)

## Snowflake.Client
.NET client for [Snowflake](https://www.snowflake.com) REST API.  
Provides API to execute SQL queries and map response to your models.  
Read my [blog post](https://medium.com/@fixer_m/better-net-client-for-snowflake-db-ecb48c48c872) about the ideas behind it. 

### Main Features
- User/Password authentication
- Execute any SQL queries with parameters
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

// Executes query with parameters (several syntax options):
var employees1 = await snowflakeClient.QueryAsync<Employee>("SELECT * FROM EMPLOYEES WHERE TITLE = ?", "Programmer");
var employees2 = await snowflakeClient.QueryAsync<Employee>("SELECT * FROM EMPLOYEES WHERE ID IN (?,?)", new int[] { 1, 2 });
var employees3 = await snowflakeClient.QueryAsync<Employee>("SELECT * FROM EMPLOYEES WHERE TITLE = :Title", new Employee() { Title = "Programmer" });
var employees4 = await snowflakeClient.QueryAsync<Employee>("SELECT * FROM EMPLOYEES WHERE TITLE = :Title", new { Title = "Programmer" });
```

### Comparison with Snowflake.Data 
Official [Snowflake.Data](https://github.com/snowflakedb/snowflake-connector-net) connector implements ADO.NET interfaces (IDbConnection, IDataReader etc), so you have to work with it as with usual database, however under the hood it actually uses Snowflake REST API. In contrast Snowflake.Client is designed as REST API client wrapper with convenient API. [Read more](https://medium.com/@fixer_m/better-net-client-for-snowflake-db-ecb48c48c872) about it.

Improvements in Snowflake.Client vs Snowflake.Data: 
- Performance: Re-uses Snowflake session, i.e. ~3x less roundtrips to SF
- Performance: Doesn't have additional intermediate mapping from SF to DB types 
- Better API: Clean and simple API vs verbose ADO.NET 
- Less third party dependencies: 0 vs 4

Added features in Snowflake.Client vs Snowflake.Data:
- Map response data to entities
- Supports `describeOnly` flag
- Has option to return raw data from Snowflake response (including QueryID and more)
- Exposes Snowflake session info 
- New SQL parameter binding API with a few options (inspired by Dapper)

Missing features in Snowflake.Client vs Snowflake.Data:
- Chunks downloader (to download massive response data) 
- OKTA Authentication

### Mapping basics
Use `QueryAsync<T>` method to get response data automatically mapped to your model (`T`): 
```csharp
// Executes query and maps response data to "Employee" class
IEnumerable<Empolyee> employees = await snowflakeClient.QueryAsync<Employee>("SELECT * FROM MASTER.PUBLIC.EMPLOYEES;");

// Your model
public class Employee
{ 
    public int Id { get; set; }
    public float? Rating { get; set; }
    public bool? IsFired { get; set; }
    public string FirstName { get; set; }
    public string[] ContactLinks { get; set; } // supports arrays and lists
    public EmplyeeInfo Info { get; set; } // supports custom json ojects ("object" and "variant")
    public DateTimeOffset HiredAt { get; set; } // DateTimeOffset for "timestamp_ltz" and "timestamp_tz"
    public DateTime FiredAt { get; set; } // DateTime for "date", "time" and "timestamp_ntz"
    public byte[] Image { get; set; } // bytes array/list for "binary"
}
```

Internally it uses [System.Text.Json](https://devblogs.microsoft.com/dotnet/try-the-new-system-text-json-apis/) to deserialize Snowflake data to your model. It uses [default deserialize behavior](https://docs.microsoft.com/ru-ru/dotnet/api/system.text.json.jsonserializer.deserialize?view=net-5.0), except `PropertyNameCaseInsensitive` is set to **true**.  
You can override this behavior by providing custom `JsonSerializerOptions`. You can pass it in `SnowflakeClient` constructor or you can set it directly via `SnowflakeDataMapper.SetJsonMapperOptions(jsonSerializerOptions)`.

### Explicit mapping 
You may want to get raw response from Snowflake, for example, to get **QueryID** or some other information.  
In this case you can use mapper explicitly: 
```csharp
// Executes query and returns raw response from Snowflake (rows, columns and query information)
var queryDataResponse = await snowflakeClient.QueryRawResponseAsync("SELECT * FROM MASTER.PUBLIC.EMPLOYEES;");

// Maps Snowflake rows and columns to your model (internally uses System.Text.Json)
var employees = SnowflakeDataMapper.MapTo<Employee>(queryDataResponse.Columns, queryDataResponse.Rows);
```

### Installation
Add nuget package [Snowflake.Client](https://www.nuget.org/packages/Snowflake.Client) to your project:  
```{r, engine='bash', code_block_name}
PM> Install-Package Snowflake.Client
```

### Road Map 
- [Done] Async API 
- [Done] Auto-renew session
- [Done] Query cancellation
- [In Progress] Unit tests
- [In Progress] Integration tests
- Better mapper documentation
- Chunks downloader (for big amount of data)
- ? Get/Put files to Stage
