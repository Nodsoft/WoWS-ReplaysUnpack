using Nodsoft.WowsReplaysUnpack.Core.Models;

namespace Nodsoft.WowsReplaysUnpack.Services;

/// <summary>
/// Specifies a service for unpacking World of Warships replay files.
/// </summary>
public interface IReplayUnpackerService<out TReplay> where TReplay : UnpackedReplay
{
	/// <summary>
	/// Unpacks a replay file (in the form of a byte array) into a <see cref="UnpackedReplay"/> object.
	/// </summary>
	/// <param name="data">The buffered replay file.</param>
	/// <param name="options">Options to use when unpacking the replay.</param>
	/// <returns>The unpacked replay.</returns>
	TReplay Unpack(byte[] data, ReplayUnpackerOptions? options = null);
	
	/// <summary>
	/// Unpacks a replay file (in the form of a data stream) into a <see cref="UnpackedReplay"/> object.
	/// </summary>
	/// <param name="stream">The streamed replay file.</param>
	/// <param name="options">Options to use when unpacking the replay.</param>
	/// <returns>The unpacked replay.</returns>
	TReplay Unpack(Stream stream, ReplayUnpackerOptions? options = null);
}