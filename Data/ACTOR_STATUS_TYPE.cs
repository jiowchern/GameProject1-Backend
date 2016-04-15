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

        KNOCKOUT1,
        CHEST_OPEN,
        CHEST_CLOSE,
        LONG_IDLE,



        MAKE,
        MELEE_IDLE,

        MELEE_ATTACK1,
        MELEE_ATTACK2,
        MELEE_ATTACK3,

        TWO_HAND_SWORD_IDLE,
        TWO_HAND_SWORD_RUN,
        TWO_HAND_SWORD_ATTACK1,

        MELEE_ATTACK4,     
        
        TIRED   ,
        STUN,
        AID,
        TWO_HAND_SWORD_ATTACK2,
    }
}
