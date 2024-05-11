using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Nodsoft.WowsReplaysUnpack.Controllers;
using Nodsoft.WowsReplaysUnpack.Core.Abstractions;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Models;
using Nodsoft.WowsReplaysUnpack.Services;

namespace Nodsoft.WowsReplaysUnpack;

/// <summary>
/// Provides a fluent API to build a WOWS replay data unpacker.
/// </summary>
[PublicAPI]
public class ReplayUnpackerBuilder
{
	private bool replayDataParserAdded;
	private bool definitionStoreAdded;
	private bool definitionLoaderAdded;

	public IServiceCollection Services { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ReplayUnpackerBuilder" /> class,
	/// by registering a <see cref="ReplayUnpackerService" /> as baseline.
	/// </summary>
	/// <param name="services"></param>
	public ReplayUnpackerBuilder(IServiceCollection services)
	{
		Services = services;
		AddReplayController<DefaultReplayController, UnpackedReplay>();
	}

	/// <summary>
	/// Registers a <see cref="IReplayDataParser" /> for use in the WOWS replay data unpacker.
	/// </summary>
	/// <typeparam name="TParser">The type of the replay data parser.</typeparam>
	/// <returns>The builder.</returns>
	public ReplayUnpackerBuilder WithReplayDataParser<TParser>() where TParser : class, IReplayDataParser
	{
		Services.AddTransient<IReplayDataParser, TParser>();
		replayDataParserAdded = true;
		return this;
	}

	/// <summary>
	/// Registers a <see cref="IReplayController" /> for use in the WOWS replay data unpacker.
	/// </summary>
	/// <typeparam name="TController">The type of the replay controller.</typeparam>
	/// <typeparam name="TReplay"></typeparam>
	/// <returns>The builder.</returns>
	public ReplayUnpackerBuilder AddReplayController<TController, TReplay>()
		where TController : class, IReplayController<TReplay>
		where TReplay : UnpackedReplay, new()
	{
		ServiceDescriptor[] existingControllers = Services.Where(s =>
				s.ServiceType.IsGenericType &&
				s.ServiceType.GetGenericTypeDefinition() == typeof(IReplayController<>))
			.ToArray();

		foreach (ServiceDescriptor existingController in existingControllers)
		{
			if (existingController.ServiceType.GenericTypeArguments[0] == typeof(TReplay))
				throw new Exception("There can only be one controller per replay type registered");
		}
		
		Services.AddScoped<IReplayUnpackerService<TReplay>, ReplayUnpackerService<TReplay>>();
		Services.AddScoped<IReplayController<TReplay>, TController>();
		return this;
	}

	/// <summary>
	/// Registers a <see cref="IDefinitionLoader" /> for use in the WOWS replay data unpacker.
	/// </summary>
	/// <typeparam name="TLoader">The type of the definition loader.</typeparam>
	/// <returns>The builder.</returns>
	public ReplayUnpackerBuilder WithDefinitionLoader<TLoader>() where TLoader : class, IDefinitionLoader
	{
		Services.AddSingleton<IDefinitionLoader, TLoader>();
		definitionLoaderAdded = true;
		return this;
	}

	/// <summary>
	/// Registers a <see cref="IDefinitionStore" /> for use in the WOWS replay data unpacker.
	/// </summary>
	/// <typeparam name="TStore">The type of the definition store.</typeparam>
	/// <returns>The builder.</returns>
	public ReplayUnpackerBuilder WithDefinitionStore<TStore>() where TStore : class, IDefinitionStore
	{
		Services.AddSingleton<IDefinitionStore, TStore>();
		definitionStoreAdded = true;
		return this;
	}
	
	/// <summary>
	/// Builds the WOWS replay data unpacker, registering any missing services.
	/// </summary>
	public void Build()
	{
		if (!replayDataParserAdded)
		{
			WithReplayDataParser<DefaultReplayDataParser>();
		}

		if (!definitionStoreAdded)
		{
			WithDefinitionStore<DefaultDefinitionStore>();
		}

		if (!definitionLoaderAdded)
		{
			WithDefinitionLoader<AssemblyDefinitionLoader>();
		}
	}
}