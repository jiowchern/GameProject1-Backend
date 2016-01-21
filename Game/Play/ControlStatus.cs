using System;
using System.Linq;

using Regulus.Framework;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class ControlStatus : Regulus.Utility.IUpdatable , IEmotion
    {
        private readonly ISoulBinder _Binder;

        private readonly Entity _Player;

        private readonly Mover _Mover;

        private readonly Map _Map;

        private readonly StageMachine _Status;

        

        public ControlStatus(ISoulBinder binder, Entity player, Mover mover , Map map)
        {
            _Binder = binder;
            _Player = player;
            _Mover = mover;
            _Map = map;
            _Status = new StageMachine();
        }

        void IBootable.Launch()
        {
            _Binder.Bind<IEmotion>(this);
            _ToDone();
        }

        private void _ToDone()
        {            
            var status = new NormalStatus(_Binder, _Player);
            status.ExploreEvent += _ToExplore;
            status.BattleEvent += _ToCast;
            status.MakeEvent += _ToMake;
            _SetStatus(status);
        }

        private void _ToMake()
        {
            var status = new MakeStatus(_Binder, _Player);
            status.DoneEvent += _ToDone;
            _SetStatus(status);
        }

       

        private void _ToCast(SkillCaster caster)
        {
            var status = new BattleCasterStatus(_Binder, _Player, _Map , caster);
            status.NextEvent += _ToCast;
            status.BattleIdleEvent += _ToBattle;
            status.DisarmEvent += _ToDone;
            _SetStatus(status);
        }

        private void _SetStatus(IStage status)
        {            
            _Status.Push(status);
        }

        private void _ToExplore(Guid obj)
        {
            var status = new ExploreStatus(_Binder, _Player , _Map , obj);
            status.DoneEvent += _ToDone;
            _SetStatus(status);
        }

        void IBootable.Shutdown()
        {
            _Status.Termination();
            _Binder.Unbind<IEmotion>(this);
        }

        bool IUpdatable.Update()
        {
            _Status.Update();
            this._ProcessDamage();
            return true;
        }

        private void _ProcessDamage()
        {
            var casters = _Player.HaveDamage();

            if (casters > 2)
                _ToKnockout();
            else if (casters > 0)
                _ToDamage();
        }

        private void _ToKnockout()
        {
            var skill = Resource.Instance.FindSkill(ACTOR_STATUS_TYPE.KNOCKOUT1);
            var caster = new SkillCaster(skill, new Determination(skill));
            var stage = new BattleCasterStatus(_Binder, _Player, _Map, caster);
            stage.BattleIdleEvent += _ToBattle;
            _SetStatus(stage);
        }

        private void _ToBattle()
        {
            var status = new BattleCasterStatus(_Binder, _Player, _Map, _Player.GetBattleCaster());
            status.NextEvent += _ToCast;
            status.DisarmEvent += _ToDone;
            _SetStatus(status);
        }

        private void _ToDamage()
        {
            var skill = Resource.Instance.FindSkill(ACTOR_STATUS_TYPE.DAMAGE1);
            var caster = new SkillCaster(skill , new Determination(skill));
            var stage = new BattleCasterStatus(_Binder , _Player , _Map , caster);
            stage.BattleIdleEvent += _ToBattle;
            _SetStatus(stage);
        }

        void IEmotion.Talk(string message)
        {
            _Player.Talk(message);
        }
    }
}