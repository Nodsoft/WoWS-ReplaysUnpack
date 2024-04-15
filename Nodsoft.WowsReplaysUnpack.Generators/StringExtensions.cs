namespace Nodsoft.WowsReplaysUnpack.Generators;

public static class StringExtensions
{
	public static string StripQuotes(this string value)
		=> value.Substring(1, value.Length - 2);

}