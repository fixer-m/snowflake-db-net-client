using System.Text;
using BenchmarkDotNet.Attributes;
using Snowflake.Client.Helpers;

namespace Snowflake.Client.Benchmarks;

[MemoryDiagnoser]
public class HexUtilsBenchmarks
{
    public static readonly string __hexCharsLong = GenerateRandomHex(2_000_000);

    private MemoryStream _memoryStream;
    private StreamWriter _streamWriter;

    [GlobalSetup]
    public void IterationSetup()
    {
        _memoryStream = new MemoryStream(1_000_000);
        _streamWriter = new StreamWriter(_memoryStream);
    }

    [Benchmark]
    public void HexToBase64_Short()
    {
        HexUtils.HexToBase64("0a0b0c", _streamWriter);
        _streamWriter.Flush();
        _memoryStream.SetLength(0);
    }

    [Benchmark]
    public void HexToBase64_Long()
    {
        HexUtils.HexToBase64(__hexCharsLong, _streamWriter);
        _streamWriter.Flush();
        _memoryStream.SetLength(0);
    }

    private static string GenerateRandomHex(int length)
    {
        Random rng = new(123);

        StringBuilder sb = new(length);
        for(int i = 0; i < length; i++)
        {
            sb.Append(rng.Next(0, 16).ToString("X"));
        }

        return sb.ToString();
    }
}
