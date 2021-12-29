using FluentAssertions;
using Nodsoft.WowsReplaysUnpack;
using Nodsoft.WowsReplaysUnpack.Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Nodsoft.WoWsReplaysUnpack.UnitTests;

[TestFixture]
public class ReplayLoading
{
	[TestCaseSource(nameof(FindTestFiles))]
	public void ReadReplay(string filename)
	{
		Stream replayStream = File.OpenRead(filename);
		ReplayUnpacker unpacker = new();

		ReplayRaw replay = unpacker.UnpackReplay(replayStream);

		replay.ReplayMetadata.ArenaInfo.Vehicles.Should().HaveCount(24);
	}

	public static IEnumerable<string> FindTestFiles()
	{
		string currentPath = Directory.GetCurrentDirectory();
		string testFileDirectory = Path.Combine(currentPath, "TestSamples");
		return Directory.GetFiles(testFileDirectory);
	}
}