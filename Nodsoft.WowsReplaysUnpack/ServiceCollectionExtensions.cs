using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Services;

namespace Nodsoft.WowsReplaysUnpack;

/// <summary>
/// Various extension methods for Dependency Injection.
/// </summary>
[PublicAPI]
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Registers the WOWS replay data unpacker,
	/// using the <see cref="DefaultReplayDataParser"/> and <see cref="DefaultDefinitionStore"/>
	/// for parsing the replay data and definitions.
	/// </summary>
	/// <param name="services">The service collection.</param>
	/// <returns>The service collection.</returns>
	public static IServiceCollection AddWowsReplayUnpacker(this IServiceCollection services)
		=> services.AddWowsReplayUnpacker<DefaultReplayDataParser, DefaultDefinitionStore, AssemblyDefinitionLoader>();

	/// <summary>
	/// Registers the WOWS replay data unpacker,
	/// using the specified <see cref="IReplayDataParser"/>, <see cref="IDefinitionStore"/> and <see cref="IDefinitionLoader"/>
	/// for parsing the replay data and definitions.
	/// </summary>
	/// <param name="services">The service collection to add the services to.</param>
	/// <typeparam name="TReplayDataParser">The type of the replay data parser.</typeparam>
	/// <typeparam name="TDefinitionStore">The type of the definition store.</typeparam>
	/// <typeparam name="TDefinitionLoader">The type of the definition loader.</typeparam>
	/// <returns>The service collection.</returns>
	public static IServiceCollection AddWowsReplayUnpacker<TReplayDataParser, TDefinitionStore, TDefinitionLoader>(
		this IServiceCollection services)
		where TReplayDataParser : class, IReplayDataParser
		where TDefinitionStore : class, IDefinitionStore
		where TDefinitionLoader : class, IDefinitionLoader
		=> services.AddWowsReplayUnpacker(unpacker =>
			{
				unpacker
					.WithReplayDataParser<TReplayDataParser>()
					.WithDefinitionStore<TDefinitionStore>()
					.WithDefinitionLoader<TDefinitionLoader>();
			}
		);

	/// <summary>
	/// Builds the WOWS replay data unpacker using the <see cref="ReplayUnpackerFactory"/>,
	/// and a builder action to configure the unpacker.
	/// </summary>
	/// <param name="services">The service collection to add the services to.</param>
	/// <param name="builderAction">The builder action to configure the unpacker.</param>
	/// <returns>The service collection.</returns>
	public static IServiceCollection AddWowsReplayUnpacker(this IServiceCollection services,
		Action<ReplayUnpackerBuilder> builderAction)
	{
		ReplayUnpackerBuilder builder = new(services);
		builderAction(builder);
		builder.Build();

		services.AddScoped<ReplayUnpackerFactory>();
		services.AddScoped<IReplayUnpackerFactory>(s => s.GetRequiredService<ReplayUnpackerFactory>());
		return services;
	}
}