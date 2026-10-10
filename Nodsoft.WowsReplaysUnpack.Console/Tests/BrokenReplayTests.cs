using Microsoft.Extensions.DependencyInjection;
using Nodsoft.WowsReplaysUnpack.ExtendedData;
using Nodsoft.WowsReplaysUnpack.Services;
using System.IO;

namespace Nodsoft.WowsReplaysUnpack.Console.Tests;

public class BrokenReplayTests
{
	public void Execute()
	{
		var services = new ServiceCollection();
		services.AddLogging();
		services.AddWowsReplayUnpacker(b => b.AddExtendedData());
		var sp = services.BuildServiceProvider();

		var path = "/mnt/data/Downloads/20261010_150030_PRSC310-Petropavlovsk_16_OC_bees_to_honey.wowsreplay";
		// var path =
		// 	@"E:\Downloads\5915c052ee734ef68f0b6b0065a0fe52-20241027_003342_PHSD509-Groningen_45_Zigzag.wowsreplay";
		var fs = File.OpenRead(path);

		var replay = sp.GetRequiredService<IReplayUnpackerFactory>()
			.GetExtendedDataUnpacker()
			.Unpack(fs);
		
		System.Console.WriteLine();
	}
}