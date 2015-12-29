using System;

using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class BattleStatus : ActorStatus , IBattleSkill , ICastSkill
    {
        private readonly ISoulBinder _Binder;

        private readonly Entity _Player;

        private readonly Map _Map;

        private ActorSkill _Skill;

        public event Action NormalEvent;

        public event Action<SkillCaster> CasterEvent;

        private readonly MoveController _MoveController;

        public BattleStatus(ISoulBinder binder, Entity player, Map map) 
        {
            _Binder = binder;
            _Player = player;
            _Map = map;
            _Skill = _Player.FindSkill();
            _MoveController = new MoveController(_Player );
        }

        public override void Enter()
        {
            _Player.Battle();
            _Binder.Bind<IBattleSkill>(this);
            _Binder.Bind<ICastSkill>(this);
            _Binder.Bind<IMoveController>(_MoveController);
        }

        public override void Leave()
        {
            _Binder.Unbind<ICastSkill>(this);
            _Binder.Unbind<IBattleSkill>(this);
            _Binder.Unbind<IMoveController>(_MoveController);
        }

        public override void Update()
        {
            
        }

        public ACTOR_STATUS_TYPE Id { get {return ACTOR_STATUS_TYPE.BATTLE_AXE_IDLE;} }

        ACTOR_STATUS_TYPE[] ICastSkill.Skills { get { return _Skill.Skills; } }
    

        void ICastSkill.Cast(ACTOR_STATUS_TYPE skill)
        {
            var caster = _Skill.FindCaster(skill);
            if (caster != null)
            {
                CasterEvent(caster);
            }
        }

        void IBattleSkill.Disarm()
        {
            NormalEvent();
        }
    }
}