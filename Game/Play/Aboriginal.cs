using System;

using Regulus.Framework;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class Aboriginal : Regulus.Utility.IUpdatable
    {
        private readonly Map _Map;

        private readonly Entity _Actor;

        private readonly Regulus.Utility.StageMachine _Machine;

        private readonly Regulus.Utility.Updater _Updater;

        private readonly Wisdom _Wisdom;
        public Aboriginal(Map map, Entity actor ,  Wisdom wisdom)
        {
            _Wisdom = wisdom;
            _Updater = new Updater();
            _Map = map;
            _Actor = actor;
            _Machine = new StageMachine();
        }

        public IIndividual Entity { get { return _Actor; } }

        void IBootable.Launch()
        {
            var provider = new ItemProvider();            
            _Actor.Bag.Add(provider.BuildItem(1 , "AidKit" , new [] {new ItemEffect()
            {
                Effects = new []{ new Effect() { Type =  EFFECT_TYPE.AID , Value = 10 } },
                Quality = 0.5f                                 
            } }));
            _ToGame(_Map);
        }

        private void _ToGame(Map map)
        {            
            var stage = new GameStage(_Wisdom.GetSoulBinder() ,  map , _Actor , _Wisdom);
            stage.DoneEvent += ()=> { _ToGame(map);  } ;
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