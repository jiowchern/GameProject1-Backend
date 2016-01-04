namespace Regulus.Project.ItIsNotAGame1.Data
{
    [ProtoBuf.ProtoContract]
    public struct Effect
    {
        public EFFECT_TYPE Type;
        public float Value;
    }

    public enum EFFECT_TYPE
    {
        ATTACK_ADD,
    }
}