using JetBrains.Annotations;
using Nodsoft.WowsReplaysUnpack.Controllers;
using Nodsoft.WowsReplaysUnpack.Services;

namespace Nodsoft.WowsReplaysUnpack;

[PublicAPI]
public static class ReplayUnpackerFactoryExtensions
{
	/// <summary>
	/// Gets the cve check only unpacker service from a <see cref="IReplayUnpackerFactory"/>.
	/// </summary>
	/// <param name="factory">The factory.</param>
	/// <returns>The cve check only unpacker service.</returns>
	public static IReplayUnpackerService<CveCheckOnlyController.CveCheckReplay> GetCveCheckUnpacker(
		this IReplayUnpackerFactory factory) => factory
		.GetUnpacker<CveCheckOnlyController.CveCheckReplay>();
}