// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Nodsoft.WowsReplaysUnpack.Benchmark;

BenchmarkRunner.Run<UnpackBenchmark>(DefaultConfig.Instance
	.WithOptions(ConfigOptions.DisableOptimizationsValidator)
	.AddJob(Job.Default.WithRuntime(CoreRuntime.Core80)));

Console.ReadLine();
