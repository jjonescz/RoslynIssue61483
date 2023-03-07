using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

public partial class Program
{
	static void Main(string[] args)
	{
		var job = Job.ShortRun;
		BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, ManualConfig.CreateMinimumViable()
			.HideColumns("Error", "StdDev", "Median", "RatioSD")
			.AddDiagnoser(new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig(printSource: true)))
			.AddJob(job.WithCustomBuildConfiguration("ReleaseCustomRoslyn"))
			.AddJob(job.AsBaseline()));
	}

	[Benchmark, Arguments(1, 2)]
	public int Compare1(int x, int y)
	{
		if (x < y) return -1;
		if (x > y) return 1;
		return 0;
	}

	[Benchmark, Arguments(1, 2)]
	public int Compare2(int x, int y)
	{
		int tmp1 = (x > y) ? 1 : 0;
		int tmp2 = (x < y) ? 1 : 0;
		return tmp1 - tmp2;
	}

	[Benchmark, Arguments('A')]
	public int Compare3(char c)
	{
		return (c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z') ? 1 : 0;
	}
}
