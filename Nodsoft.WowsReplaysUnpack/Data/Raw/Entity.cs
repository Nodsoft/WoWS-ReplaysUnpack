using Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser;
using System.IO;

namespace Nodsoft.WowsReplaysUnpack.Data.Raw;

public class Entity
{
	public Entity(long id)
	{
		Id = id;
	}

	public long Id { get; }
	
	public object? CrewParameters { get; private set; }

	public void SetClientProperty(int index, MemoryStream payload)
	{
	}

	private void ProcessCrewModifiers(MemoryStream payload)
	{
		
	}
}