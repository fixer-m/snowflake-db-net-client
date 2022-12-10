[![NuGet](https://img.shields.io/badge/nuget-v0.4.3-green.svg)](https://www.nuget.org/packages/Snowflake.Client/) 
[![](https://img.shields.io/nuget/dt/Snowflake.Client.svg)](https://www.nuget.org/packages/Snowflake.Client/) 
[![Targets](https://img.shields.io/badge/.NET%20Standard-2.0-green.svg)](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) 
[![License](https://img.shields.io/badge/License-Apache%202.0-green.svg)](https://opensource.org/licenses/Apache-2.0)

## Snowflake.Client
.NET client for [Snowflake](https://www.snowflake.com) REST API.  
Provides API to execute SQL queries and map response to your models.  
Read my [blog post](https://medium.com/@fixer_m/better-net-client-for-snowflake-db-ecb48c48c872) about the ideas behind it. 

### Installation
Add nuget package [Snowflake.Client](https://www.nuget.org/packages/Snowflake.Client) to your project:  
```{r, engine='bash', code_block_name}
PM> Install-Package Snowflake.Client
```

### Main features
- User/Password authentication
- Execute any SQL queries with parameters
- Map response data to your models

### Basic usage
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
```

### Comparison with Snowflake.Data 
Official [Snowflake.Data](https://github.com/snowflakedb/snowflake-connector-net) connector implements ADO.NET interfaces (IDbConnection, IDataReader etc), so you have to work with it as with usual database, however under the hood it actually uses Snowflake REST API. In contrast Snowflake.Client is designed as REST API client wrapper with convenient API. [Read more](https://medium.com/@fixer_m/better-net-client-for-snowflake-db-ecb48c48c872) about it.

Improvements in Snowflake.Client vs Snowflake.Data: 
- Performance: Re-uses Snowflake session, i.e. **~3x less roundtrips to SF**
- Performance: Doesn't have additional intermediate mapping from SF to DB types 
- Better API: Clean and simple API vs verbose ADO.NET 
- No third party dependencies vs 9 external packages in Snowflake.Data

New features in Snowflake.Client:
- Map response data to entities
- Supports `describeOnly` flag
- Has option to return raw data from Snowflake response (including QueryID and more)
- Exposes Snowflake session info 
- New SQL parameter binding API with a few options (inspired by Dapper)

Missing features in Snowflake.Client:
- Authentication options other than basic user/password
- GET/PUT command (was recenlty implemented in Snowflake.Data)

### Parameter binding
Snowflake supports two placeholder formats for [parameter binding](https://docs.snowflake.com/en/user-guide/python-connector-example.html#qmark-or-numeric-binding):
- Positional — with a "?" placeholders 
- Named — parameter name prefixed with a ":"

Both formats are supported. You can use positional placeholders to bind values of "simple" types (like `Int`, `String` or `DateTime`). To bind named parameters you can use classes, structs, anonymous types or dictionary. See examples below. 
```csharp
// Positional placeholder, any "simple" type
var result1 = await snowflakeClient.QueryAsync<Employee>
              ("SELECT * FROM EMPLOYEES WHERE TITLE = ?", "Programmer");

// Positional placeholders, any IEnumerable<T>
var result2 = await snowflakeClient.QueryAsync<Employee>
              ("SELECT * FROM EMPLOYEES WHERE ID IN (?, ?, ?)", new int[] { 1, 2, 3 });

// Named placeholders, any custom class or struct
var result3 = await snowflakeClient.QueryAsync<Employee>  
              ("SELECT * FROM EMPLOYEES WHERE TITLE = :Title", new Employee() { Title = "Programmer" });

// Named placeholders, any anonymous class
var result4 = await snowflakeClient.QueryAsync<Employee>     
              ("SELECT * FROM EMPLOYEES WHERE TITLE = :Title", new { Title = "Junior" });

// Named placeholders, any IDictionary<T>
var result5 = await snowflakeClient.QueryAsync<Employee>
              ("SELECT * FROM EMPLOYEES WHERE TITLE = :Title", new Dictionary<string, string> {{ "Title", "Programmer" }});
```

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

Internally it uses [System.Text.Json](https://devblogs.microsoft.com/dotnet/try-the-new-system-text-json-apis/) to deserialize Snowflake data to your model. It uses [default deserialize behavior](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-5-0#deserialization-behavior), except `PropertyNameCaseInsensitive` is set to **true**, so your properties names don't have to be in the exact same case as column names in your tables.  
You can override this behavior by providing custom `JsonSerializerOptions`. You can pass it in `SnowflakeClient` constructor or you can set it directly via `SnowflakeDataMapper.SetJsonMapperOptions(jsonSerializerOptions)`.

### Advanced usage 
You may want to get raw response from Snowflake, for example, to get **QueryID** or some other information.  
In this case you can use mapper explicitly: 
```csharp
// Executes query and returns raw response from Snowflake (rows, columns and query information)
var queryDataResponse = await snowflakeClient.QueryRawResponseAsync("SELECT * FROM MASTER.PUBLIC.EMPLOYEES;");

// Maps Snowflake rows and columns to your model (internally uses System.Text.Json)
var employees = SnowflakeDataMapper.MapTo<Employee>(queryDataResponse.Columns, queryDataResponse.Rows);
```
You can override internal http client. Fr example, this can be used to bypass SSL check:
```csharp
var handler = new HttpClientHandler
{
    SslProtocols = SslProtocols.Tls12,
    CheckCertificateRevocationList = false, 
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // i.e. bypass cert validation
};

var httpClient = new HttpClient(handler);
var snowflakeClient = new SnowflakeClient("user", "password", "account", "region");
snowflakeClient.SetHttpClient(httpClient);
```

### Release notes

0.4.0
- Increased http client tiemout to 1 hour for a long running queries
- Added missing cancellation token for a few methods

0.3.9
- Now can handle [long-running queries](https://github.com/fixer-m/snowflake-db-net-client/issues/15) (> 45 seconds)
- Now returns date-time values as `DateTimeKind.Unspecified`

0.3.8
- Implemented [downloading big data reponses](https://github.com/fixer-m/snowflake-db-net-client/issues/13) (> 1000 rows) from chunks (`ChunksDownloader`)
- Now returns affected rows count for `COPY UNLOAD` command

0.3.7
- Added cancellation token for public async methods
- Improved mapping tests

0.3.6
- Set `Expect100Continue` and `UseNagleAlgorithm` to false for better HTTP performance
- Allow streaming for http responses with `ResponseHeadersRead` option
- Improved bool mapping
- Adding `IDictionary<>` support for binding parameters

0.3.5
- Added response auto-decompression
- Added cloud tag auto-detection to finally fix [SSL cert issue](https://github.com/fixer-m/snowflake-db-net-client/issues/7)
- Fix: explicit URL host now actually have higher priority than auto-constructed
- Now it automatically replaces underscore in account name 
- [More info on this release](https://github.com/fixer-m/snowflake-db-net-client/issues/7#issuecomment-812715944)

0.3.4
- Forced TLS 1.2 and revocation check as in official connector

0.3.3
- Added `SetHttpClient()` as workaround for [SSL cert issue](https://github.com/fixer-m/snowflake-db-net-client/issues/7)

0.3.2
- Added support for binding from class fields and structs
- Added a lot of unit tests 
- Started working on integration tests
- Now uses it's own driver name _.NET_Snowflake.Client_

0.3.1
- Implemented query cancellation with `CancelQueryAsync()`
- Fixed [issue with mapping](https://github.com/fixer-m/snowflake-db-net-client/issues/4#issue-795843806) for semi-structured SF types (object, variant, array)
- Implemented auto-renewing SF session, if its expired
- Initializes SF session automatically with first query
- `QueryRawAsync()` now returns response with all metadata
- Extracted interfaces for public classes 

0.3.0
- Changed all API methods to be async
- Added a lot of documentation in readme file
- Implemented first unit tests
- Target frameworks changed to NET Standard 2.0 and .NET Core 3.1

0.2.4
- Fix: now actually uses passed `JsonMapperOptions`
- New `Execute()` method which returns affected rows count

0.2.3
- Changed return type of `ExecuteScalar()` to string
- Added comments and documentation for public methods

0.2.2
- First public release
- Implemented all basic features
