using System;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    [ProtoBuf.ProtoContract][Serializable]
    public class Energy
    {
        public enum TYPE
        {
            HEALTH_DECREASE,
        }
        [ProtoBuf.ProtoMember(1)]
        public TYPE Type;
        [ProtoBuf.ProtoMember(2)]
        public float Value;
    }
}