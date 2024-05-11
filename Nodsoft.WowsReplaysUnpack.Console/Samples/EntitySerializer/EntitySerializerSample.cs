using Microsoft.Extensions.DependencyInjection;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.EntitySerializer;
using Nodsoft.WowsReplaysUnpack.ExtendedData;
using Nodsoft.WowsReplaysUnpack.ExtendedData.Models;
using Nodsoft.WowsReplaysUnpack.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Nodsoft.WowsReplaysUnpack.Console.Samples.EntitySerializer;

public class EntitySerializerSample : TaskBase
{
	protected override void ConfigureServices(IServiceCollection serviceCollection)
	{
		serviceCollection.AddWowsReplayUnpacker(builder =>
		{
			builder.AddExtendedData();
		});
	}

	protected override Task ExecuteAsync(IServiceProvider services)
	{
		using IReplayUnpackerService<ExtendedDataReplay> unpacker =
			services.GetRequiredService<IReplayUnpackerFactory>().GetExtendedDataUnpacker();

		ExtendedDataReplay alphaReplay = unpacker.Unpack(GetReplayFile("press_account_alpha.wowsreplay"));
		ExtendedDataReplay bravoReplay = unpacker.Unpack(GetReplayFile("press_account_bravo.wowsreplay"));

		ManualExtensions(alphaReplay, bravoReplay);
		Serializer(alphaReplay, bravoReplay);

		return Task.CompletedTask;
	}

	private void ManualExtensions(UnpackedReplay alphaReplay, UnpackedReplay bravoReplay)
	{
		FixedList? alphaState = alphaReplay.Entities.Single(e => e.Value.Name == "BattleLogic").Value.ClientProperties
			.GetAsDict("state")?
			.GetAsDict("missions")?
			.GetAsArr("teamsScore");

		FixedList? bravoState = bravoReplay.Entities.Single(e => e.Value.Name == "BattleLogic").Value.ClientProperties
			.GetAsDict("state")?
			.GetAsDict("missions")?
			.GetAsArr("teamsScore");

		ushort? alphaScoreA = alphaState?.GetAsDict(0)?.GetAsValue<ushort>("score");
		ushort? alphaScoreB = alphaState?.GetAsDict(1)?.GetAsValue<ushort>("score");

		ushort? bravoScoreA = bravoState?.GetAsDict(0)?.GetAsValue<ushort>("score");
		ushort? bravoScoreB = bravoState?.GetAsDict(1)?.GetAsValue<ushort>("score");

		System.Console.WriteLine("Manuel Extensions:");
		System.Console.WriteLine($"Alpha Replay: [{alphaScoreA}:{alphaScoreB}]");
		System.Console.WriteLine($"Bravo Replay: [{bravoScoreA}:{bravoScoreB}]");
	}

	private void Serializer(UnpackedReplay alphaReplay, UnpackedReplay bravoReplay)
	{
		BattleLogic alphaBattleLogic = alphaReplay.DeserializeEntity<BattleLogic>("BattleLogic");
		BattleLogic bravoBattleLogic = bravoReplay.DeserializeEntity<BattleLogic>("BattleLogic");


		System.Console.WriteLine("Manuel Extensions:");
		System.Console.WriteLine(
			$"Alpha Replay: [{alphaBattleLogic.State.Missions.TeamsScore[0].Score}:{alphaBattleLogic.State.Missions.TeamsScore[1].Score}]");
		System.Console.WriteLine(
			$"Bravo Replay: [{bravoBattleLogic.State.Missions.TeamsScore[0].Score}:{bravoBattleLogic.State.Missions.TeamsScore[1].Score}]");
	}
}