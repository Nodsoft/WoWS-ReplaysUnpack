using Nodsoft.WowsReplaysUnpack.ExtendedData.Models;
using Nodsoft.WowsReplaysUnpack.Services;

namespace Nodsoft.WowsReplaysUnpack.ExtendedData;

public static class Extensions
{
	/// <summary>
	/// Gets the extended data unpacker service from a <see cref="ReplayUnpackerFactory"/>.
	/// </summary>
	/// <param name="factory">The factory.</param>
	/// <returns>The extended data unpacker service.</returns>
	public static IReplayUnpackerService<ExtendedDataReplay> GetExtendedDataUnpacker(this ReplayUnpackerFactory factory) => factory
		.GetUnpacker<ExtendedDataReplay>();
}
