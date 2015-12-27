using System;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    [ProtoBuf.ProtoContract]
    public struct Item
    {
       [ProtoBuf.ProtoMember(1)]
        public Guid Id { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public int Weight    { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public string Name { get; set; }
    }
}