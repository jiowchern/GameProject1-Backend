namespace Regulus.Project.ItIsNotAGame1.Data
{
    [ProtoBuf.ProtoContract]
    public enum ACTOR_STATUS_TYPE
    {
        NORMAL_IDLE,
        NORMAL_EXPLORE,
        BATTLE_AXE_IDLE,
        BATTLE_AXE_ATTACK1,        
        BATTLE_AXE_ATTACK2,
        BATTLE_AXE_BLOCK,
        DAMAGE1,

        GUARD_IMPACT,

        KNOCKOUT1
    }
}