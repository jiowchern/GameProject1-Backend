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
        MELEE_ATTACK4,

        CLAYMORE_IDLE,
        CLAYMORE_RUN,
        CLAYMORE_ATTACK1,
        CLAYMORE_ATTACK2,
        
        
        TIRED   ,
        STUN,
        AID,
        


        SWORD_IDLE,
        SWORD_RUN,
        SWORD_ATTACK1,
        SWORD_ATTACK2,
        SWORD_ATTACK3,

        DUALSWORD_IDLE,
        DUALSWORD_RUN,
        DUALSWORD_ATTACK1,
        DUALSWORD_ATTACK2,
        DUALSWORD_ATTACK3,


    }
}
