// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Nodsoft.WowsReplaysUnpack.Console;
using Nodsoft.WowsReplaysUnpack.Data;



/*
BenchmarkRunner.Run<UnpackBenchmark>(DefaultConfig.Instance
	.WithOptions(ConfigOptions.DisableOptimizationsValidator)
);

/**/


ReplayRaw replay = new UnpackBenchmark().GetReplay();
replay.RefreshRelatedEntities();

/**/


foreach (ReplayMessage msg in replay.ChatMessages)
{
	Console.WriteLine($"[{GetGroupString(msg)}] {msg.Player?.Name ?? msg.EntityId.ToString()} : {msg.MessageContent}");
}

/**/

/*
Console.ReadKey();

/**/

static string GetGroupString(ReplayMessage msg) => msg.MessageGroup switch
{
	"battle_team" => "Team",
	"battle_common" => "All",
	_ => ""
};

/**/