using Nodsoft.WowsReplaysUnpack.Core.Models;

namespace Nodsoft.WowsReplaysUnpack.Services;

/// <summary>
/// Represents a factory for creating <see cref="IReplayUnpackerService{TReplay}"/> instances.
/// </summary>
public interface IReplayUnpackerFactory
{
	/// <summary>
	/// Gets an <see cref="ReplayUnpackerService{TReplay}" /> with the specified <typeparamref name="TReplay"/>.
	/// </summary>
	/// <typeparam name="TReplay">The type of the controller.</typeparam>
	IReplayUnpackerService<TReplay> GetUnpacker<TReplay>() where TReplay : UnpackedReplay;

	/// <summary>
	/// Gets the default <see cref="ReplayUnpackerService{UnpackedReplay}" />.
	/// </summary>
	/// <returns>An instance of <see cref="IReplayUnpackerService{UnpackedReplay}" />.</returns>
	IReplayUnpackerService<UnpackedReplay> GetUnpacker();
}