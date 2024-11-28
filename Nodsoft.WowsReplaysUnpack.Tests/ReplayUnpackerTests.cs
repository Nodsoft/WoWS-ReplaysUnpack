using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Exceptions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.ExtendedData;
using Nodsoft.WowsReplaysUnpack.ExtendedData.Models;
using Nodsoft.WowsReplaysUnpack.Services;
using Xunit;

/*
* FIXME: Test parallelization is disabled due to a file loading issue.
*/
[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Nodsoft.WowsReplaysUnpack.Tests;

public sealed class ReplayUnpackerTests
{
	private readonly string _sampleFolder = Path.Join(Directory.GetCurrentDirectory(), "Replay-Samples");

	public ReplayUnpackerTests()
	{ }

	
	/// <summary>
	/// Test parsing a working replay using the default controller.
	/// </summary>
	[
		Theory,
		InlineData("0.10.10.wowsreplay"),
		InlineData("0.10.11.wowsreplay"),
		InlineData("0.11.0.wowsreplay"),
		InlineData("0.11.2.wowsreplay"),
		InlineData("12.6.wowsreplay"),
		InlineData("12.7.wowsreplay"),
		InlineData("12.8.wowsreplay"),
		InlineData("12.9.wowsreplay"),
		InlineData("12.10.wowsreplay"),
		InlineData("12.10_2.wowsreplay"),
		InlineData("12.11.1.wowsreplay"),
		InlineData("13.0.wowsreplay"),
		InlineData("13.0.1.wowsreplay"),
		InlineData("13.1.wowsreplay"),
		InlineData("13.2.wowsreplay"),
		InlineData("13.3.wowsreplay"),
		InlineData("13.4.wowsreplay"),
		InlineData("13.5.wowsreplay"),
		InlineData("13.5.1.wowsreplay"),
		InlineData("13.6.wowsreplay"),
		InlineData("13.6.1.wowsreplay"),
		InlineData("13.7.wowsreplay"),
		InlineData("13.8.wowsreplay"),
		InlineData("13.9.wowsreplay"),
		InlineData("13.10.wowsreplay"),
		InlineData("13.11.wowsreplay"),
	]
	public void TestReplay_Pass(string replayPath)
	{
		using MemoryStream ms = new();

		using (FileStream fs = File.OpenRead(Path.Join(_sampleFolder, replayPath)))
		{
			fs.CopyTo(ms);
			Assert.Equal(fs.Length, ms.Length);
		}
		
		ms.Position = 0;

		ReplayUnpackerFactory replayUnpackerFactory = new ServiceCollection()
			.AddLogging(l => l.ClearProviders())
			.AddWowsReplayUnpacker()
			.BuildServiceProvider()
			.GetRequiredService<ReplayUnpackerFactory>();

		UnpackedReplay replay = replayUnpackerFactory.GetUnpacker().Unpack(ms);
		
		Assert.NotNull(replay);
	}
	
	/// <summary>
	/// Test parsing a working replay using the ExtendedData controller.
	///	- This controller is not registered by default, so we need to register it manually.
	/// </summary>
	[
		Theory,
		InlineData("0.10.10.wowsreplay"),
		InlineData("0.10.11.wowsreplay"),
		InlineData("0.11.0.wowsreplay"),
		InlineData("0.11.2.wowsreplay"),
		InlineData("12.6.wowsreplay"),
		InlineData("12.7.wowsreplay"),
		InlineData("12.8.wowsreplay"),
		InlineData("12.9.wowsreplay"),
		InlineData("12.10.wowsreplay"),
		InlineData("12.10_2.wowsreplay"),
		InlineData("12.11.1.wowsreplay"),
		InlineData("13.0.wowsreplay"),
		InlineData("13.0.1.wowsreplay"),
		InlineData("13.1.wowsreplay"),
		InlineData("13.2.wowsreplay"),
		InlineData("13.3.wowsreplay"),
		InlineData("13.4.wowsreplay"),
		InlineData("13.5.wowsreplay"),
		InlineData("13.5.1.wowsreplay"),
		InlineData("13.6.wowsreplay"),
		InlineData("13.6.1.wowsreplay"),
		InlineData("13.7.wowsreplay"),
		InlineData("13.8.wowsreplay"),
		InlineData("13.9.wowsreplay"),
		InlineData("13.10.wowsreplay"),
		InlineData("13.11.wowsreplay"),
	]
	public void TestReplay_ExtendedData_Pass(string replayPath)
	{
		using MemoryStream ms = new();

		using (FileStream fs = File.OpenRead(Path.Join(_sampleFolder, replayPath)))
		{
			fs.CopyTo(ms);
			Assert.Equal(fs.Length, ms.Length);
		}
		
		ms.Position = 0;

		ReplayUnpackerFactory replayUnpackerFactory = new ServiceCollection()
			.AddLogging(l => l.ClearProviders())
			.AddWowsReplayUnpacker(builder =>
			{
				builder.AddExtendedData();
			})
			.BuildServiceProvider()
			.GetRequiredService<ReplayUnpackerFactory>();

		UnpackedReplay replay = replayUnpackerFactory.GetExtendedDataUnpacker().Unpack(ms);
		
		Assert.NotNull(replay);
		Assert.IsType<ExtendedDataReplay>(replay);
	}
}