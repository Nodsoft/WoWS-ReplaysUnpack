using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nodsoft.WowsReplaysUnpack.Generators.ReplayController;

internal static class ReplayControllerSourceWriter
{
	public static string NewMethod(ControllerToGenerate controller)
	{
		IEnumerable<IGrouping<string, MethodSubscriptionData>> methods = controller.Methods.GroupBy(m => $"{m.EntityName}_{m.MethodName}");

		StringBuilder sb = new StringBuilder(SourceGenerationHelper.Header).AppendLine();
		sb.AppendLine("using Nodsoft.WowsReplaysUnpack.Core.Entities;");
		sb.Append("namespace ").Append(controller.Namespace).AppendLine(";").AppendLine();

		sb.Append("public partial class ").Append(controller.ClassName).AppendLine();
		sb.AppendLine("{").AppendLine();

		bool isBaseController =
			controller.FullyQualifiedName is "Nodsoft.WowsReplaysUnpack.Controllers.ReplayControllerBase<T>";

		string methodModifier = isBaseController ? "virtual" : "override";

		if (controller.Methods.Count > 0)
		{
			sb.AppendLine(
				$"  public {methodModifier} void CallSubscription(string hash, Entity entity, float packetTime, Dictionary<string, object?> arguments)");
			sb.AppendLine("  {");

			if (!isBaseController)
			{
				sb.AppendLine("    base.CallSubscription(hash, entity, packetTime, arguments);");
			}

			sb.AppendLine("    switch (hash)");
			sb.AppendLine("    {");

			foreach (IGrouping<string, MethodSubscriptionData>? methodGroup in methods)
			{
				sb.AppendLine($"      case \"{methodGroup.Key}\":");
				foreach (MethodSubscriptionData sub in methodGroup.OrderBy(m => m.Priority ?? 999))
				{
					sb.Append("        ").Append(sub.CallName).Append("(");

					if (sub.IncludeEntity)
						sb.Append("entity, ");

					if (sub.IncludePacketTime)
						sb.Append("packetTime, ");

					if (sub.ParamsAsDictionary)
					{
						sb.AppendLine("arguments);");
					}
					else
					{
						for (int i = 0; i < sub.ParameterTypes.Count; i++)
						{
							string type = sub.ParameterTypes.ElementAt(i);
							string postFix = type.EndsWith("?") ? string.Empty: "!";
							sb.Append($"({sub.ParameterTypes.ElementAt(i)})arguments.Values.ElementAt({i}){postFix}");
							if (i < sub.ParameterTypes.Count - 1)
								sb.Append(", ");
						}

						sb.AppendLine(");");
					}
				}

				sb.AppendLine("        break;");
			}

			sb.AppendLine("    }");
			sb.AppendLine("  }");
		}

		sb.AppendLine("}");

		return sb.ToString();
	}
}