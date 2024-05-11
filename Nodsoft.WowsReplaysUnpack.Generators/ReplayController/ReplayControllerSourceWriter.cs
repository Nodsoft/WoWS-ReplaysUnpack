using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nodsoft.WowsReplaysUnpack.Generators.ReplayController;

internal static class ReplayControllerSourceWriter
{
	public static string Generate(ControllerToGenerate controller)
	{
		IEnumerable<IGrouping<string, MethodSubscriptionData>> methods =
			controller.Methods.GroupBy(m => $"{m.EntityName}_{m.MethodName}");

		IEnumerable<IGrouping<string, PropertyChangedSubscriptionData>> properties =
			controller.Properties.GroupBy(m => $"{m.EntityName}_{m.PropertyName}");

		StringBuilder sb = new StringBuilder(SourceGenerationHelper.Header).AppendLine();
		sb.AppendLine("using Nodsoft.WowsReplaysUnpack.Core.Entities;");
		sb.Append("namespace ").Append(controller.Namespace).AppendLine(";").AppendLine();

		sb.Append("public partial class ").Append(controller.ClassName).AppendLine();
		sb.AppendLine("{").AppendLine();

		bool isBaseController =
			controller.FullyQualifiedName is "Nodsoft.WowsReplaysUnpack.Controllers.ReplayControllerBase<T>";

		string methodModifier = isBaseController ? "virtual" : "override";

		WriteMethodSubscriptions(sb, methodModifier, isBaseController, methods);
		WritePropertyChangedSubscriptions(sb, methodModifier, isBaseController, properties);

		sb.AppendLine("}");

		return sb.ToString();
	}

	private static void WriteMethodSubscriptions(StringBuilder sb,
		string methodModifier,
		bool isBaseController,
		IEnumerable<IGrouping<string, MethodSubscriptionData>> methods)
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
				sb.Append("        ").Append(sub.CallName).Append('(');

				if (sub.IncludeEntity)
				{
					sb.Append("entity, ");
				}

				if (sub.IncludePacketTime)
				{
					sb.Append("packetTime, ");
				}

				if (sub.ParamsAsDictionary)
				{
					sb.AppendLine("arguments);");
				}
				else
				{
					for (int i = 0; i < sub.ParameterTypes.Count; i++)
					{
						string type = sub.ParameterTypes.ElementAt(i);
						string postFix = type.EndsWith("?") ? string.Empty : "!";
						sb.Append($"({sub.ParameterTypes.ElementAt(i)})arguments.Values.ElementAt({i}){postFix}");
						if (i < sub.ParameterTypes.Count - 1)
						{
							sb.Append(", ");
						}
					}

					sb.AppendLine(");");
				}
			}

			sb.AppendLine("        break;");
		}

		sb.AppendLine("    }");
		sb.AppendLine("  }");
		sb.AppendLine();
	}

	private static void WritePropertyChangedSubscriptions(StringBuilder sb,
		string methodModifier,
		bool isBaseController, IEnumerable<IGrouping<string, PropertyChangedSubscriptionData>> methods)
	{
		sb.AppendLine(
			$"  public {methodModifier} void PropertyChanged(string hash, Entity entity, object? value)");
		sb.AppendLine("  {");

		if (!isBaseController)
		{
			sb.AppendLine("    base.PropertyChanged(hash, entity, value);");
		}

		sb.AppendLine("    switch (hash)");
		sb.AppendLine("    {");

		foreach (IGrouping<string, PropertyChangedSubscriptionData>? methodGroup in methods)
		{
			sb.AppendLine($"      case \"{methodGroup.Key}\":");
			foreach (PropertyChangedSubscriptionData sub in methodGroup)
			{
				sb.Append("        ").Append(sub.CallName).Append("(entity, ");
				string postFix = sub.ParameterType.EndsWith("?") ? string.Empty : "!";
				sb.Append($"({sub.ParameterType})value{postFix}");
				sb.AppendLine(");");
			}

			sb.AppendLine("        break;");
		}

		sb.AppendLine("    }");
		sb.AppendLine("  }");
		sb.AppendLine();
	}
}