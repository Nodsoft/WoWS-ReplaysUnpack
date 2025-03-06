using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.ExtendedData;
using Nodsoft.WowsReplaysUnpack.ExtendedData.Models;
using Nodsoft.WowsReplaysUnpack.Services;
using System.Diagnostics.Contracts;
using Xunit;

/*
* FIXME: Test parallelization is disabled due to a file loading issue.
*/
[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Nodsoft.WowsReplaysUnpack.Tests;

public sealed class ReplayUnpackerTests
{
	private static readonly string _sampleFolder = Path.Join(Directory.GetCurrentDirectory(), "Replay-Samples");
	
	/// <summary>
	/// Test parsing a working replay using the default controller.
	/// </summary>
	[Theory, MemberData(nameof(GetReplayFileList))]
	public void TestReplay_Pass(string replayPath)
	{
		using MemoryStream ms = new();

		using (FileStream fs = File.OpenRead(Path.Join(_sampleFolder, replayPath)))
		{
			fs.CopyTo(ms);
			Assert.Equal(fs.Length, ms.Length);
		}
		
		ms.Position = 0;

		IReplayUnpackerFactory replayUnpackerFactory = new ServiceCollection()
			.AddLogging(l => l.ClearProviders())
			.AddWowsReplayUnpacker()
			.BuildServiceProvider()
			.GetRequiredService<IReplayUnpackerFactory>();

		UnpackedReplay replay = replayUnpackerFactory.GetUnpacker().Unpack(ms);
		
		Assert.NotNull(replay);
	}
	
	/// <summary>
	/// Test parsing a working replay using the ExtendedData controller.
	///	- This controller is not registered by default, so we need to register it manually.
	/// </summary>
	[Theory, MemberData(nameof(GetReplayFileList))]
	public void TestReplay_ExtendedData_Pass(string replayPath)
	{
		using MemoryStream ms = new();

		using (FileStream fs = File.OpenRead(Path.Join(_sampleFolder, replayPath)))
		{
			fs.CopyTo(ms);
			Assert.Equal(fs.Length, ms.Length);
		}
		
		ms.Position = 0;

		IReplayUnpackerFactory replayUnpackerFactory = new ServiceCollection()
			.AddLogging(l => l.ClearProviders())
			.AddWowsReplayUnpacker(builder =>
			{
				builder.AddExtendedData();
			})
			.BuildServiceProvider()
			.GetRequiredService<IReplayUnpackerFactory>();

		UnpackedReplay replay = replayUnpackerFactory.GetExtendedDataUnpacker().Unpack(ms);
		
		Assert.NotNull(replay);
		Assert.IsType<ExtendedDataReplay>(replay);
	}

	public static IEnumerable<object[]> GetReplayFileList()
	{
		// Version replays
		yield return ["0.10.10.wowsreplay"];
		yield return ["0.10.11.wowsreplay"];
		yield return ["0.11.0.wowsreplay"];
		yield return ["0.11.2.wowsreplay"];
		yield return ["12.6.wowsreplay"];
		yield return ["12.7.wowsreplay"];
		yield return ["12.8.wowsreplay"];
		yield return ["12.9.wowsreplay"];
		yield return ["12.10.wowsreplay"];
		yield return ["12.10_2.wowsreplay"];
		yield return ["12.11.1.wowsreplay"];
		yield return ["13.0.wowsreplay"];
		yield return ["13.0.1.wowsreplay"];
		yield return ["13.1.wowsreplay"];
		yield return ["13.2.wowsreplay"];
		yield return ["13.3.wowsreplay"];
		yield return ["13.4.wowsreplay"];
		yield return ["13.5.wowsreplay"];
		yield return ["13.5.1.wowsreplay"];
		yield return ["13.6.wowsreplay"];
		yield return ["13.6.1.wowsreplay"];
		yield return ["13.7.wowsreplay"];
		yield return ["13.8.wowsreplay"];
		yield return ["13.9.wowsreplay"];
		yield return ["13.10.wowsreplay"];
		yield return ["13.11.wowsreplay"];
		yield return ["14.0.wowsreplay"];
		yield return ["14.1.wowsreplay"];
		yield return ["14.2.wowsreplay"];

		// Special edge cases
		yield return ["Spike1.wowsreplay"];
	}
}