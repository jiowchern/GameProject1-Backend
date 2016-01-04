using System;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    [ProtoBuf.ProtoContract]
    public struct Item
    {
       [ProtoBuf.ProtoMember(1)]
        public Guid Id { get; set; }
        
        [ProtoBuf.ProtoMember(2)]
        public string Name { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public int Weight { get; set; }
        [ProtoBuf.ProtoMember(4)]

        public Effect[] Effects { get; set; }

        [ProtoBuf.ProtoMember(5)]
        public int Count { get; set; }

    }
}