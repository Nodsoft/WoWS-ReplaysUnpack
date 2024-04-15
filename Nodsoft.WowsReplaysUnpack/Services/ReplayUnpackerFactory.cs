using Microsoft.Extensions.DependencyInjection;
using Nodsoft.WowsReplaysUnpack.Controllers;
using Nodsoft.WowsReplaysUnpack.Core.Models;

namespace Nodsoft.WowsReplaysUnpack.Services;

/// <summary>
/// Represents a factory for creating <see cref="IReplayUnpackerService"/> instances.
/// </summary>
public class ReplayUnpackerFactory
{
	private readonly IServiceProvider _serviceProvider;

	public ReplayUnpackerFactory(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

	/// <summary>
	/// Gets an <see cref="ReplayUnpackerService{TReplay}" /> with the specified <typeparamref name="TReplay"/>.
	/// </summary>
	/// <typeparam name="TReplay">The type of the controller.</typeparam>
	public IReplayUnpackerService<TReplay> GetUnpacker<TReplay>() where TReplay : UnpackedReplay
		=> _serviceProvider.GetRequiredService<ReplayUnpackerService<TReplay>>();

	/// <summary>
	/// Gets the default <see cref="ReplayUnpackerService{UnpackedReplay}" />.
	/// </summary>
	/// <returns>An instance of <see cref="IReplayUnpackerService{UnpackedReplay}" />.</returns>
	public IReplayUnpackerService<UnpackedReplay> GetUnpacker() => GetUnpacker<UnpackedReplay>();
}
