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

      
        private readonly Regulus.Utility.StageMachine _Machine;

        private readonly Regulus.Utility.Updater _Updater;
        public Aboriginal(Zone zone)
        {
            
            _Updater = new Updater();
            _Zone = zone;
            _Machine = new StageMachine();
        }

        void IBootable.Launch()
        {
            
            Map map = this._Zone.FindMap("test");
            _ToGame(map);
        }

        private void _ToGame(Map map)
        {
            var actor = EntityProvider.Create(ENTITY.ACTOR2);
            var wisdom = new Wisdom(actor);
            var stage = new GameStage(wisdom.GetSoulBinder() ,  map , actor , wisdom);
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