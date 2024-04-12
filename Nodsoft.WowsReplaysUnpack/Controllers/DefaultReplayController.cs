using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Nodsoft.WowsReplaysUnpack.Core.Definitions;
using Nodsoft.WowsReplaysUnpack.Core.Entities;
using Nodsoft.WowsReplaysUnpack.Core.Models;

namespace Nodsoft.WowsReplaysUnpack.Controllers;

/// <summary>
/// Default implementation of the <see cref="ReplayControllerBase{TController}"/>.
/// </summary>
[UsedImplicitly]
public sealed class DefaultReplayController : ReplayControllerBase<UnpackedReplay>
{
	// ReSharper disable once ContextualLoggerProblem
	public DefaultReplayController(IDefinitionStore definitionStore, ILogger<Entity> entityLogger) : base(definitionStore, entityLogger) { }
}