using System;

using ProtoBuf;

using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    [ProtoContract]
    [Serializable]
    public struct EntityData
    {
        [ProtoMember(1)]
        public ENTITY Name;
        [ProtoMember(2)]
        public Polygon Mesh;
        
    }
}