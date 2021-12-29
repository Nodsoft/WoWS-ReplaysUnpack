using System.IO;

namespace Nodsoft.WoWsReplaysUnpack.UnitTests;

public static class TestHelper
{
	private static readonly string currentPath = Directory.GetCurrentDirectory();

	public static string GetTestFilePath(string filename) => Path.Combine(currentPath, "TestSamples", filename);
}