namespace Regulus.Project.ItIsNotAGame1.Data
{
    [ProtoBuf.ProtoContract]
    public struct Effect
    {
        [ProtoBuf.ProtoMember(1)]
        public EFFECT_TYPE Type;
        [ProtoBuf.ProtoMember(2)]
        public float Value;
    }

    public enum EFFECT_TYPE
    {
        ATTACK_ADD,
        ILLUMINATE_ADD,
        LIFE,
    }
}