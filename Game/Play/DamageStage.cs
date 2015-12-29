using System;

using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class DamageStage : IStage
    {
        private readonly ISoulBinder _Binder;

        private readonly Entity _Player;

        private readonly SkillCaster _Caster;

        public event Action DoneEvent;

        public DamageStage(ISoulBinder binder, Entity player)
        {
            _Binder = binder;
            _Player = player;
            _Caster = _Player.GetDamagrCaster();
        }

        void IStage.Enter()
        {
            _Player.Damage();
        }

        void IStage.Leave()
        {
            
        }

        void IStage.Update()
        {
            if (_Caster.IsDone())
                DoneEvent();
        }

               
    }
}