using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodsoft.WowsReplaysUnpack;

public static class GlobalConstants
{
	public const string BlowfishKey = "\x29\xB7\xC9\x09\x38\x3F\x84\x88\xFA\x98\xEC\x4E\x13\x19\x79\xFB";

	public static class ReplayPacketTypes
	{
		public const byte OnEntityCreate = 0x5;
		public const byte OnEntityMethod = 0x8;
	}
}
