using System.Text;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.ObjectPool;
using Snowflake.Client.Helpers;
using Snowflake.Client.ObjectPool;

namespace Snowflake.Client.Benchmarks;

[MemoryDiagnoser]
public class HexUtilsBenchmarks
{
    static readonly string __hexCharsLong = GenerateRandomHex(2_000_000);

    static readonly ObjectPool<StringBuilder> __stringBuilderPool =
        new DefaultObjectPool<StringBuilder>(
            new CustomStringBuilderPooledObjectPolicy());

    [Benchmark]
    public void HexToBase64_Short()
    {
        var sb = __stringBuilderPool.Get();
        try
        {
            HexUtils.HexToBase64("0a0b0c", sb);
        }
        finally
        {
            __stringBuilderPool.Return(sb);
        }
    }

    [Benchmark]
    public void HexToBase64_Long()
    {
        var sb = __stringBuilderPool.Get();
        try
        {
            HexUtils.HexToBase64(__hexCharsLong, sb);
        }
        finally
        {
            __stringBuilderPool.Return(sb);
        }
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
