using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;

[DisassemblyDiagnoser(printSource: true)]
[HideColumns("Error", "StdDev", "Median", "RatioSD")]
public partial class Program
{
	static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

    [Benchmark, ArgumentsSource(nameof(GetArguments))]
    public int Compare1(int x, int y)
    {
        if (x < y) return -1;
        if (x > y) return 1;
        return 0;
    }

    [Benchmark, ArgumentsSource(nameof(GetArguments))]
    public int Compare2(int x, int y)
    {
        int tmp1 = (x > y) ? 1 : 0;
        int tmp2 = (x < y) ? 1 : 0;
        return tmp1 - tmp2;
    }

    public IEnumerable<object[]> GetArguments()
    {
        var rnd = new Random(42);
        return Enumerable.Range(0, 1).Select(i => new object[] { rnd.Next(), rnd.Next() });
    }
}
