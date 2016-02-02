using System;

using Regulus.Framework;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class Aboriginal : Regulus.Utility.IUpdatable
    {
        private readonly Zone _Zone;

        private readonly Entity _Actor;

        private readonly Regulus.Utility.StageMachine _Machine;

        private readonly Regulus.Utility.Updater _Updater;

        private Wisdom _Wisdom;
        public Aboriginal(Zone zone , Entity actor ,  Wisdom wisdom)
        {
            _Wisdom = wisdom;
            _Updater = new Updater();
            _Zone = zone;
            _Actor = actor;
            _Machine = new StageMachine();
        }

        void IBootable.Launch()
        {            
            Map map = this._Zone.FindMap("test");
            _ToGame(map);
        }

        private void _ToGame(Map map)
        {
            
            var stage = new GameStage(_Wisdom.GetSoulBinder() ,  map , _Actor , _Wisdom);
            //stage.DoneEvent += _Idle ;
            _Machine.Push(stage);

            
        }

        void IBootable.Shutdown()
        {
            _Updater.Shutdown();
        }

        bool IUpdatable.Update()
        {
            _Updater.Working();
            _Machine.Update();
            return true;

        }

        
    }
}