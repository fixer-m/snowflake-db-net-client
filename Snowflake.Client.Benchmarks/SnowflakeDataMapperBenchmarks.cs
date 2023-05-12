using BenchmarkDotNet.Attributes;
using Snowflake.Client.Json;

namespace Snowflake.Client.Benchmarks;

[MemoryDiagnoser]
public class SnowflakeDataMapperBenchmarks
{
    readonly QueryExecResponseData _responseSample;
    readonly List<List<string>> _objectRowset;

    public SnowflakeDataMapperBenchmarks()
    {
        _responseSample = GetFakeResponse();
        _objectRowset = new List<List<string>>() { _responseSample.RowSet[0] };
    }

    [Benchmark]
    public void ResponseWithValues_MapTo_CustomClass()
    {
        var enumerable = SnowflakeDataMapper.MapTo<CustomClass>(_responseSample.RowType, _objectRowset);

        // Enumerate the result to actually execute the code inside the iterator method.
        string stringValue;
        foreach(var result in enumerable)
        {
            stringValue = result.StringProperty;
        }
    }

    private static QueryExecResponseData GetFakeResponse()
    {
        var response = new QueryExecResponseData() { RowType = new List<ColumnDescription>(), RowSet = new List<List<string>>() };

        response.RowType.Add(new ColumnDescription() { Name = "StringProperty", Type = "text" });
        response.RowType.Add(new ColumnDescription() { Name = "BoolProperty", Type = "boolean" });
        response.RowType.Add(new ColumnDescription() { Name = "IntProperty", Type = "fixed" });
        response.RowType.Add(new ColumnDescription() { Name = "FloatProperty", Type = "real" });
        response.RowType.Add(new ColumnDescription() { Name = "DecimalProperty", Type = "real" });
        response.RowType.Add(new ColumnDescription() { Name = "DateTimeProperty", Type = "timestamp_ntz" });
        response.RowType.Add(new ColumnDescription() { Name = "DateTimeOffsetProperty", Type = "timestamp_ltz" });
        response.RowType.Add(new ColumnDescription() { Name = "GuidProperty", Type = "text" });
        response.RowType.Add(new ColumnDescription() { Name = "ByteArrayProperty", Type = "binary" });

        for(int i=0; i < 10; i++)
        { 
            response.RowSet.Add(
                new List<string>()
                {
                    "Sometext",
                    "true",
                    "7",
                    "27.6",
                    "19.239834",
                    "1600000000.000000000",
                    "1600000000.000000000",
                    "e7412bbf-88ee-4149-b341-101e0f72ec7c",
                    "0080ff0a0b0c0d0e0f"
                });
        }

        response.RowSet.Add(
            new List<string>()
            {
                "null",
                "null",
                "null",
                "null",
                "null",
                "null",
                "null",
                "null",
                "null"
            });

        response.RowSet.Add(
            new List<string>()
            {
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            });

        return response;
    }

    public class CustomClass
    {
        public string StringProperty { get; set; }

        public bool BoolProperty { get; set; }

        public int IntProperty { get; set; }

        public float FloatProperty { get; set; }

        public decimal DecimalProperty { get; set; }

        public DateTime DateTimeProperty { get; set; }

        public DateTimeOffset DateTimeOffsetProperty { get; set; }

        public Guid GuidProperty { get; set; }

        public byte[] ByteArrayProperty { get; set; }
    }
}
