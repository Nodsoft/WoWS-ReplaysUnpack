using Microsoft.Extensions.DependencyInjection;
using Nodsoft.WowsReplaysUnpack.ExtendedData.Models;
using Nodsoft.WowsReplaysUnpack.ExtendedData.VersionMappings;
using static Nodsoft.WowsReplaysUnpack.ServiceCollectionExtensions;

namespace Nodsoft.WowsReplaysUnpack.ExtendedData;

/// <summary>
/// Various extension methods for Dependency Injection.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Adds support for extended data on a replay unpacker builder.
	/// </summary>
	/// <param name="builder">The replay unpacker builder.</param>
	/// <returns>The replay unpacker builder.</returns>
	public static ReplayUnpackerBuilder AddExtendedData(this ReplayUnpackerBuilder builder)
	{
		builder.AddReplayController<ExtendedDataController, ExtendedDataReplay>();
		builder.Services.AddSingleton<VersionMappingFactory>();

		foreach (Type? versionMappingType in VersionMappingFactory.VersionMappingTypes)
		{
			builder.Services.AddSingleton(typeof(IVersionMapping), versionMappingType);
		}

		return builder;
	}
}