using System.Text;
using BenchmarkDotNet.Attributes;
using Snowflake.Client.Helpers;

namespace Snowflake.Client.Benchmarks;

[MemoryDiagnoser]
public class HexUtilsBenchmarks
{
    static readonly string __hexCharsLong = GenerateRandomHex(2_000_000);

    [Benchmark]
    public void HexToBytes_Short()
    {
        var sb = new StringBuilder();
        HexUtils.HexToBase64("0a0b0c", sb);
    }

    [Benchmark]
    public void HexToBytes_Long()
    {
        var sb = new StringBuilder();
        HexUtils.HexToBase64(__hexCharsLong, sb);
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
