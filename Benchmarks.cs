using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Program
{
	static void Main(string[] args)
	{
		var jobs =
			from runtime in new[] { CoreRuntime.Core70, CoreRuntime.Core80 }
			from config in new[] { "Release", "ReleaseCustomRoslyn" }
			select Job.Default.WithRuntime(runtime).WithCustomBuildConfiguration(config)
				.WithBaseline(runtime == CoreRuntime.Core80 && config == "Release");

		BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, ManualConfig.CreateMinimumViable()
			.AddLogicalGroupRules(BenchmarkLogicalGroupRule.ByMethod)
			.HideColumns("Error", "StdDev", "Median", "RatioSD", "x", "y", "c")
			.AddDiagnoser(new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig(
				printSource: true,
				exportHtml: true,
				exportCombinedDisassemblyReport: true,
				exportDiff: true)))
			.AddJob(jobs.ToArray()));
	}

	private int _x, _y;
	private char _c;
	public IEnumerable<object[]> Xy => new[] { new object[] { _x, _y } };
	public IEnumerable<object[]> C => new[] { new object[] { _c } };

	[GlobalSetup]
	public void Setup()
	{
		_x = Random.Shared.Next();
		_y = Random.Shared.Next();
		_c = (char)Random.Shared.Next(255);
	}

	[Benchmark, ArgumentsSource(nameof(Xy))]
	public int Compare1(int x, int y)
	{
		if (x < y) return -1;
		if (x > y) return 1;
		return 0;
	}

	[Benchmark, ArgumentsSource(nameof(Xy))]
	public int Compare2(int x, int y)
	{
		int tmp1 = (x > y) ? 1 : 0;
		int tmp2 = (x < y) ? 1 : 0;
		return tmp1 - tmp2;
	}

	[Benchmark, ArgumentsSource(nameof(C))]
	public int Compare3(char c)
	{
		return (c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z') ? 1 : 0;
	}
}
