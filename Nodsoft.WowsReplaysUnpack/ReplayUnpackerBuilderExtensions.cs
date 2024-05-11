using JetBrains.Annotations;
using Nodsoft.WowsReplaysUnpack.Controllers;

namespace Nodsoft.WowsReplaysUnpack;

[PublicAPI]
public static class ReplayUnpackerBuilderExtensions
{
	public static ReplayUnpackerBuilder AddCveCheckController(this ReplayUnpackerBuilder builder)
		=> builder.AddReplayController<CveCheckOnlyController, CveCheckOnlyController.CveCheckReplay>();
}