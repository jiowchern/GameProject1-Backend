using System;

using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class StunStatus : IStage
    {
        private readonly ISoulBinder _Binder;

        private readonly Entity _Player;

        private readonly Regulus.Utility.TimeCounter _Counter;

        public Action SurvivalEvent;

        public StunStatus(ISoulBinder binder, Entity player)
        {
            _Binder = binder;
            _Player = player;

            _Counter = new TimeCounter();
        }

        void IStage.Enter()
        {
            _Player.Stun();
            _Counter.Reset();
        }

        void IStage.Leave()
        {            
        }

        void IStage.Update()
        {
            if (_Counter.Second > 60f)
            {
                SurvivalEvent();
            }
        }
    }
}