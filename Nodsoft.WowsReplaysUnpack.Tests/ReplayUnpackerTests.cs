using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Exceptions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
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
	/// Test parsing a working replay (Assets/good.wowsreplay)
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
}