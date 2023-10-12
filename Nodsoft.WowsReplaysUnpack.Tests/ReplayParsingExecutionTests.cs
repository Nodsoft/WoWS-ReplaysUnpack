using Microsoft.Extensions.DependencyInjection;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.FileStore.Definitions;
using Nodsoft.WowsReplaysUnpack.Services;
using Xunit;


namespace Nodsoft.WowsReplaysUnpack.Tests;

/// <summary>
/// Provides integration tests for the different replay parser configurations.
/// </summary>
public class ReplayParsingExecutionTests
{
	/// <summary>
	/// Tests the parsing of a replay file, using the default configuration.
	/// </summary>
	[Fact]
	public void TestDefaultParsing()
	{
		// Arrange
		IServiceProvider services = new ServiceCollection()
			.AddLogging()
			.AddWowsReplayUnpacker()
			.BuildServiceProvider();
		
		ReplayUnpackerFactory unpackerFactory = services.GetRequiredService<ReplayUnpackerFactory>();
		IReplayUnpackerService service = unpackerFactory.GetUnpacker();
		
		// Act
		UnpackedReplay metadata = service.Unpack(Utilities.LoadReplay("good.wowsreplay"));
		
		// Assert
		Assert.NotNull(metadata);
	}
	
	/// <summary>
	/// Tests the parsing of a replay file, using the filesystem defs loader.
	/// </summary>
	/// <remarks>
	/// This test requires the filesystem to be setup correctly, and definitions to be present.
	/// </remarks>
	[Fact]
	public void TestFilesystemParsing()
	{
		// Arrange
		IServiceProvider services = new ServiceCollection()
			.AddLogging()
			.AddWowsReplayUnpacker(builder => builder
				.WithDefinitionLoader<FileSystemDefinitionLoader>())
			.Configure<FileSystemDefinitionLoaderOptions>(options =>
			{
				options.RootDirectory = options.RootDirectory = Path.Join(Directory.GetCurrentDirectory(), 
					"..", "..", "..", "..", "Nodsoft.WowsReplaysUnpack.Core", "Definitions", "Versions");
			})
			.BuildServiceProvider();
		
		ReplayUnpackerFactory unpackerFactory = services.GetRequiredService<ReplayUnpackerFactory>();
		IReplayUnpackerService service = unpackerFactory.GetUnpacker();
		
		// Act
		UnpackedReplay metadata = service.Unpack(Utilities.LoadReplay("good.wowsreplay"));
		
		// Assert
		Assert.NotNull(metadata);
	}
}