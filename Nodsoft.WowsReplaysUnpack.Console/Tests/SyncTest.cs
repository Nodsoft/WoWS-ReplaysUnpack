using Microsoft.Extensions.DependencyInjection;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nodsoft.WowsReplaysUnpack.Console.Tests;

public class SyncTest : TaskBase
{
	protected override void ConfigureServices(IServiceCollection serviceCollection)
	{
		serviceCollection.AddWowsReplayUnpacker();
	}

	protected override Task ExecuteAsync(IServiceProvider services)
	{
		using IReplayUnpackerService<UnpackedReplay> unpacker =
			services.GetRequiredService<IReplayUnpackerFactory>().GetUnpacker();

		_ = RunTasks(unpacker, 20, false);

		return Task.CompletedTask;
	}

	private UnpackedReplay[] RunTasks(IReplayUnpackerService<UnpackedReplay> unpacker, int cycles,
		bool sync)
	{
		List<UnpackedReplay> unpackedReplays = new();
		if (sync)
		{
			for (int i = 0; i < cycles; i++)
			{
				unpacker.Unpack(GetReplayFile("good.wowsreplay"));
			}
		}
		else
		{
			Parallel.ForEach(Enumerable.Range(0, cycles), _ =>
			{
				unpackedReplays.Add(unpacker.Unpack(GetReplayFile("good.wowsreplay")));
			});
		}

		return [..unpackedReplays];
	}
}