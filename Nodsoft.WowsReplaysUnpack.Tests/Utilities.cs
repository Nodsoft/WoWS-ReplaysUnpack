using System.Runtime.CompilerServices;

namespace Nodsoft.WowsReplaysUnpack.Tests;

/// <summary>
/// Helper methods used in tests.
/// </summary>
public static class Utilities
{
	private static readonly string _sampleFolder = Path.Join(Directory.GetCurrentDirectory(), "Replay-Samples");
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static MemoryStream LoadReplay(string replayPath)
	{
		using FileStream fs = File.Open(Path.Join(_sampleFolder, replayPath), FileMode.Open, FileAccess.Read, FileShare.Read);
		MemoryStream ms = new();
		fs.CopyTo(ms);
		fs.Close();
		ms.Position = 0;

		return ms;
	}
}