namespace Nodsoft.WowsReplaysUnpack.Core.Extensions;

public static class StringExtensions
{
	public static string GetStringBeforeIndex(this string str, char before) => str[..str.IndexOf(before)];

	public static string GetStringBeforeIndex(this string str, string before) => str[..str.IndexOf(before, StringComparison.Ordinal)];

	public static string GetStringAfterLength(this string str, string after) => str[after.Length..];
}
 