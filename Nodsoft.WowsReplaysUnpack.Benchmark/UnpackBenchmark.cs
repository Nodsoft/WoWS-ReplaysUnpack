using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Benchmark.Controllers;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Services;

namespace Nodsoft.WowsReplaysUnpack.Benchmark
{
	public class UnpackBenchmark
	{
		private static readonly string samplesPath = Path.Join(Environment.CurrentDirectory, "Samples");
		private static readonly MemoryStream _ms;
		private static readonly IReplayUnpackerService<UnpackedReplay> _defaultUnpacker;
		private static readonly IReplayUnpackerService<UnpackedReplay> _performanceUnpacker;

		static UnpackBenchmark()
		{
			if (_ms is null)
			{
				FileStream fs = File.OpenRead(Path.Join(samplesPath, "0.11.2.wowsreplay"));
				_ms = new();
				fs.CopyTo(_ms);
				fs.Dispose();
			}
			ServiceProvider? services = new ServiceCollection()
					.AddWowsReplayUnpacker(b =>
					{
						b.AddReplayController<PerformanceController, UnpackedReplay>();
					})
					.AddLogging(l => l.ClearProviders())
					.BuildServiceProvider();

			ReplayUnpackerFactory? factory = services.GetRequiredService<ReplayUnpackerFactory>();
			_defaultUnpacker = factory.GetUnpacker();
			_performanceUnpacker = factory.GetUnpacker<UnpackedReplay>();
		}

		[Benchmark]
		public void DefaultUnpack()
		{
			using MemoryStream ms = new(_ms.ToArray());
			_ = _defaultUnpacker.Unpack(ms);
		}

		[Benchmark]
		public void PerformanceUnpack()
		{
			using MemoryStream ms = new(_ms.ToArray());
			_ = _performanceUnpacker.Unpack(ms);
		}
	}
}
