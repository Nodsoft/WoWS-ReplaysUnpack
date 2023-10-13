using Nodsoft.WowsReplaysUnpack.Core.DataTypes;

namespace Nodsoft.WowsReplaysUnpack.Core.Models;

public class FixedList : List<object?>, IFixedLength
{
	/// <summary>
	/// Data type of the list.
	/// </summary>
	public DataTypeBase ElementType { get; }

	/// <inheritdoc />
	public int Length => Count;

	public FixedList(DataTypeBase elementType, IEnumerable<object?> values) : base(values) => ElementType = elementType;

	/// <summary>
	/// Slices the list to the given length.
	/// </summary>
	/// <param name="start">The start index.</param>
	/// <param name="end">The end index.</param>
	/// <param name="newItems">The new items to add to the list.</param>
	public void Slice(int start, int end, IEnumerable<object?> newItems)
	{
		List<object?> newList = this.Take(start).Concat(newItems).Concat(this.Skip(end)).ToList();
		Clear();
		AddRange(newList);
	}

	/// <summary>
	/// Returns a string enumeration of the list.
	/// </summary>
	/// <returns>A string enumeration of the list.</returns>
	public override string ToString() => $"[{string.Join($"{','} ", this)}]";
}
