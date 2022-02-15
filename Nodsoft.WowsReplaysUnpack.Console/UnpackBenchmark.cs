using BenchmarkDotNet.Attributes;
using Nodsoft.WowsReplaysUnpack.Data;
using System.IO;

namespace Nodsoft.WowsReplaysUnpack.Console
{
	public class UnpackBenchmark
	{
		readonly ReplayUnpacker _replayUnpacker = new();
		static readonly MemoryStream _ms;

		static UnpackBenchmark()
		{
			if (_ms is null)
			{
				FileStream fs = File.OpenRead(Path.Join(Directory.GetCurrentDirectory(), "sampleData", "10.10.wowsreplay"));
				_ms = new();
				fs.CopyTo(_ms);
				fs.Dispose();
			}
		}

		[Benchmark]
		public ReplayRaw GetReplay()
		{
			_ms.Position = 0;
			return _replayUnpacker.UnpackReplay(_ms);
		}
	}
}
