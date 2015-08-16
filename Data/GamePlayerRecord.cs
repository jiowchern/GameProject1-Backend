using System;


using ProtoBuf;

namespace Regulus.Project.ItIsNotAGame1.Data
{
	/// <summary>
	///     game player記錄資料
	/// </summary>
	[ProtoContract]
	public class GamePlayerRecord
	{
		[ProtoMember(1)]
		public Guid Id { get; set; }

		[ProtoMember(2)]
		public Guid Owner { get; set; }

        [ProtoMember(3)]

        public Guid[] Actors { get; set; }

	    public Location Start { get; set; }
	}

}

