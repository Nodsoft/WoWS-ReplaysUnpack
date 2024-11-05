using Microsoft.Extensions.DependencyInjection;
using Nodsoft.WowsReplaysUnpack.Services;
using System.IO;

namespace Nodsoft.WowsReplaysUnpack.Console.Tests;

public class BrokenReplayTests
{
	public void Execute()
	{
		var services = new ServiceCollection();
		services.AddLogging();
		services.AddWowsReplayUnpacker();
		var sp = services.BuildServiceProvider();

		var path = @"E:\Downloads\20241104_185414_PWSB719-Niord_19_OC_prey.wowsreplay";
		// var path =
		// 	@"E:\Downloads\5915c052ee734ef68f0b6b0065a0fe52-20241027_003342_PHSD509-Groningen_45_Zigzag.wowsreplay";
		var fs = File.OpenRead(path);

		var replay = sp.GetRequiredService<IReplayUnpackerFactory>()
			.GetUnpacker()
			.Unpack(fs);
		
		System.Console.WriteLine();
	}
}