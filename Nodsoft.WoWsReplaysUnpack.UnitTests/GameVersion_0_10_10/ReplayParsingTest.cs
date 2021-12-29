using FluentAssertions;
using Nodsoft.WowsReplaysUnpack;
using Nodsoft.WowsReplaysUnpack.Data;
using NUnit.Framework;
using System.IO;

namespace Nodsoft.WoWsReplaysUnpack.UnitTests.GameVersion_0_10_10;

[TestFixture]
public class ReplayParsingTest
{
	private Stream _replayStream = null!;
	private ReplayUnpacker _unpacker = null!;

	[OneTimeSetUp]
	public void ClassSetup()
	{
		_replayStream = File.OpenRead(TestHelper.GetTestFilePath("0.10.10.wowsreplay"));
	}
	
	[SetUp]
	public void Setup()
	{
		_replayStream.Position = 0;
		_unpacker = new();
	}
	
	[Test]
	public void ParseReplay_VerifyMetadata()
	{
		ReplayRaw replay = _unpacker.UnpackReplay(_replayStream);

		ArenaInfo arenaInfo = replay.ReplayMetadata.ArenaInfo;
		arenaInfo.ClientVersionFromXml.Should().StartWith("0,10,10");
		arenaInfo.Duration.Should().Be(1200);
		arenaInfo.TeamsCount.Should().Be(2);
		arenaInfo.Duration.Should().Be(arenaInfo.BattleDuration);
	}
}