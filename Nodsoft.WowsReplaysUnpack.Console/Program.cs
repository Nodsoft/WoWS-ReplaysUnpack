// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Nodsoft.WowsReplaysUnpack.Console;
using Nodsoft.WowsReplaysUnpack.Data;
using System;

Console.WriteLine();

/*
BenchmarkRunner.Run<UnpackBenchmark>(DefaultConfig.Instance
	.WithOptions(ConfigOptions.DisableOptimizationsValidator)
	.AddJob(Job.Default.WithRuntime(CoreRuntime.Core50))
	.AddJob(Job.Default.WithRuntime(CoreRuntime.Core60))
);

/**/


ReplayRaw replay = new UnpackBenchmark().GetReplay();

/**/

/*
foreach (ReplayMessage msg in replay.ChatMessages)
{
	Console.WriteLine($"[{GetGroupString(msg)}] {msg.EntityId} : {msg.MessageContent}");
}

/**/

Console.ReadKey();


#pragma warning disable CS8321
static string GetGroupString(ReplayMessage msg) => msg.MessageGroup switch
{
	"battle_team" => "Team",
	"battle_common" => "All",
	_ => ""
};
#pragma warning restore CS8321

/**/