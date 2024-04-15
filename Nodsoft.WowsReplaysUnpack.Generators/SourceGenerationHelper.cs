﻿namespace Nodsoft.WowsReplaysUnpack.Generators;

public static class SourceGenerationHelper
{
	public const string Header = @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Nodsoft.WowsReplaysUnpack.Generators source generator
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#nullable enable";


	public const string ReplayControllerAttribute = Header + @"
namespace Nodsoft.WowsReplaysUnpack.Generators;

[global::System.AttributeUsage(global::System.AttributeTargets.Class)]
internal class ReplayControllerAttribute : global::System.Attribute;";
}