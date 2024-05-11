using Microsoft.Extensions.DependencyInjection;
using Nodsoft.WowsReplaysUnpack.Core.Models;

namespace Nodsoft.WowsReplaysUnpack.Services;

/// <inheritdoc />
public class ReplayUnpackerFactory : IReplayUnpackerFactory
{
	private readonly IServiceProvider _serviceProvider;

	public ReplayUnpackerFactory(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

	/// <inheritdoc />
	public IReplayUnpackerService<TReplay> GetUnpacker<TReplay>() where TReplay : UnpackedReplay
		=> _serviceProvider.GetRequiredService<IReplayUnpackerService<TReplay>>();

	/// <inheritdoc />
	public IReplayUnpackerService<UnpackedReplay> GetUnpacker() => GetUnpacker<UnpackedReplay>();
}
