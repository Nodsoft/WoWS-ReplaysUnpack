using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Exceptions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Services;
using Xunit;

namespace Nodsoft.WowsReplaysUnpack.Tests;

/// <summary>
/// Provides tests for the replay RCE detection system.
/// </summary>
public class ReplaySanitizerTests
{
	private readonly ReplayUnpackerFactory _factory;
	

	public ReplaySanitizerTests()
	{
		_factory = new ServiceCollection()
			.AddLogging(l => l.ClearProviders())
			.AddWowsReplayUnpacker()
			.BuildServiceProvider()
			.GetRequiredService<ReplayUnpackerFactory>();
	}

	/// <summary>
	/// Test parsing a working replay (Assets/good.wowsreplay)
	/// </summary>
	[Fact]
	public void TestGoodReplay_Pass()
	{
		UnpackedReplay replay = _factory.GetUnpacker().Unpack(Utilities.LoadReplay("good.wowsreplay"));
		Assert.NotNull(replay);
	}

	/// <summary>
	/// Test malicious replay detection (Assets/payload.wowsreplay)
	/// </summary>
	[Fact]
	public void TestPayloadReplayDetection()
	{
		Assert.Throws<CveSecurityException>(() => _factory.GetUnpacker().Unpack(Utilities.LoadReplay("payload.wowsreplay")));
	}
}