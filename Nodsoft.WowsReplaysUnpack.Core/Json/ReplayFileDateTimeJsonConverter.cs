using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nodsoft.WowsReplaysUnpack.Core.Json;

/// <summary>
/// Defines a custom converter for all <see cref="DateTime"/> values encountered in a replay file's JSON.
/// </summary>
public class ReplayFileDateTimeJsonConverter : JsonConverter<DateTime>
{
	private const string DateFormat = "dd.MM.yyyy HH:mm:ss";

	/// <inheritdoc />
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!DateTime.TryParseExact(reader.GetString() ?? "", DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
		{
			// Fallback to the default DateTime converter if the string is not in the expected format.
			result = DateTime.Parse(reader.GetString() ?? "", CultureInfo.InvariantCulture);
		}

		return result;
	}

	/// <inheritdoc />
	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToString(DateFormat));
	}
}