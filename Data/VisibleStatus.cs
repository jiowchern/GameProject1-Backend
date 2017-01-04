using System;

using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    [ProtoBuf.ProtoContract][Serializable]
    public struct VisibleStatus
    {
        [ProtoBuf.ProtoMember(1)]
        public ACTOR_STATUS_TYPE Status { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public Vector2 StartPosition { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public float Speed { get; set; }
        [ProtoBuf.ProtoMember(4)]
        public float Direction { get; set; }
        [ProtoBuf.ProtoMember(5)]
        public float Trun { get; set; }

        [ProtoBuf.ProtoMember(6)]
        public Vector2 SkillOffect { get; set; }
    }
}