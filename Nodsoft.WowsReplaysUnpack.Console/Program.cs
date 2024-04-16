// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.EntitySerializer;
using Nodsoft.WowsReplaysUnpack.FileStore.Definitions;
using Nodsoft.WowsReplaysUnpack.Generators;
using Nodsoft.WowsReplaysUnpack.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Test;

string samplePath = Path.Join(Directory.GetCurrentDirectory(), "../../../../Nodsoft.WowsReplaysUnpack.Tests",
	"Replay-Samples");
FileStream _GetReplayFile(string name) => File.OpenRead(Path.Join(samplePath, name));

ServiceProvider? services = new ServiceCollection()
	//.AddWowsReplayUnpacker(builder =>
	//{
	//	//builder.AddReplayController<CVECheckOnlyController>();
	//	//builder.AddExtendedData();
	//})
	.AddWowsReplayUnpacker(builder => builder
		.WithDefinitionLoader<FileSystemDefinitionLoader>())
	.Configure<FileSystemDefinitionLoaderOptions>(options =>
	{
		options.RootDirectory = options.RootDirectory = Path.Join(Directory.GetCurrentDirectory(),
			"..", "..", "..", "..", "Nodsoft.WowsReplaysUnpack.Core", "Definitions", "Versions");
	})
	.AddLogging(logging =>
	{
		logging.ClearProviders();
		logging.AddConsole();
		logging.SetMinimumLevel(LogLevel.Error);
	})
	.BuildServiceProvider();

ReplayUnpackerFactory? replayUnpacker = services.GetRequiredService<ReplayUnpackerFactory>();

//var unpackedReplay = replayUnpacker.GetUnpacker().Unpack(GetReplayFile("payload.wowsreplay"));
//var unpackedReplay = replayUnpacker.GetUnpacker<CVECheckOnlyController>().Unpack(GetReplayFile("payload.wowsreplay"));
//ExtendedDataReplay? unpackedReplay = (ExtendedDataReplay)replayUnpacker.GetExtendedDataUnpacker().Unpack(_GetReplayFile("good.wowsreplay"));

//foreach (ReplayMessage msg in replay.ChatMessages)
//{
//	Console.WriteLine($"[{GetGroupString(msg)}] {msg.EntityId} : {msg.MessageContent}");
//}

// const int CYCLE = 20;
//
// async Task<UnpackedReplay[]> syncTasks(bool sync)
// {
// 	List<UnpackedReplay> unpackedReplays = new();
// 	if (sync)
// 	{
// 		for (int i = 0; i < CYCLE; i++)
// 		{
// 			replayUnpacker.GetUnpacker().Unpack(_GetReplayFile("good.wowsreplay"));
// 		}
// 	}
// 	else
// 	{
// 		Parallel.ForEach(Enumerable.Range(0, CYCLE), (i) =>
// 		{
// 			unpackedReplays.Add(replayUnpacker.GetUnpacker().Unpack(_GetReplayFile("good.wowsreplay")));
// 		});
// 	}
//
// 	return unpackedReplays.ToArray();
// }
//
// DateTime start = DateTime.Now;
// await syncTasks(false);
// Console.WriteLine(DateTime.Now - start);

UnpackedReplay goodReplay = replayUnpacker.GetUnpacker().Unpack(_GetReplayFile("good.wowsreplay"));
UnpackedReplay alphaReplay = replayUnpacker.GetUnpacker().Unpack(_GetReplayFile("press_account_alpha.wowsreplay"));
UnpackedReplay bravoReplay = replayUnpacker.GetUnpacker().Unpack(_GetReplayFile("unfinished_replay.wowsreplay"));

var alphaState = alphaReplay.Entities.Single(e => e.Value.Name == "BattleLogic").Value.ClientProperties
	.GetAsDict("state")?
	.GetAsDict("missions")?
	.GetAsArr("teamsScore");

var bravoState = bravoReplay.Entities.Single(e => e.Value.Name == "BattleLogic").Value.ClientProperties
	.GetAsDict("state")?
	.GetAsDict("missions")?
	.GetAsArr("teamsScore");

var scoreA = alphaState?.GetAsDict(0)?.GetAsValue<ushort>("score");
var scoreB = alphaState?.GetAsDict(1)?.GetAsValue<ushort>("score");

var _scoreA = bravoState?.GetAsDict(0)?.GetAsValue<ushort>("score");
var _scoreB = bravoState?.GetAsDict(1)?.GetAsValue<ushort>("score");


var test = alphaReplay.DeserializeEntity<BattleLogic>("BattleLogic");

Console.WriteLine();
Console.ReadKey();

namespace Test
{
	[SerializableEntity]
	public partial class BattleLogic
	{
		[DataMember(Name = "state")]
		public State State { get; set; } = null!;
	}

	public class State
	{
		[DataMember(Name = "missions")]
		public Missions Missions { get; set; } = null!;
	}

	public class Missions
	{
		[DataMember(Name = "teamsScore")]
		public List<TeamsScore> TeamsScore { get; set; } = null!;
	}

	public class TeamsScore
	{
		[DataMember(Name = "score")]
		public ushort Score { get; set; }
	}
}


//static string GetGroupString(ReplayMessage msg) => msg.MessageGroup switch
//{
//	"battle_team" => "Team",
//	"battle_common" => "All",
//	_ => ""
//};