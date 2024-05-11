using System.Diagnostics;

namespace Nodsoft.WowsReplaysUnpack.Core.Entities;


/// <summary>
/// Defines a method subscription for entity events.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
[Conditional("NODSOFT_GENERATORS_USAGES")]
public sealed class MethodSubscriptionAttribute : Attribute
{
	/// <summary>
	/// Name of the entity.
	/// </summary>
	public string EntityName { get; }

	/// <summary>
	/// Name of the entity's method.
	/// </summary>
	public string MethodName { get; }

	/// <summary>
	/// Whether method parameters should be passed as a dictionary.
	/// </summary>
	public bool ParamsAsDictionary { get; set; }

	/// <summary>
	/// Whether entity information should be included.
	/// </summary>
	public bool IncludeEntity { get; set; }

	/// <summary>
	/// Whether packet time should be included.
	/// </summary>
	public bool IncludePacketTime { get; set; }

	/// <summary>
	/// Method execution priority, where the lowest values take precedence on method calls.
	/// </summary>
	/// <remarks>
	/// <b><c>-1</c></b> is reserved for security checks.
	/// </remarks>
	public int Priority { get; set; }

	public MethodSubscriptionAttribute(string entityName, string methodName) => (EntityName, MethodName) = (entityName, methodName);
}