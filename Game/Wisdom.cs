using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Regulus.Framework;
using Regulus.Project.ItIsNotAGame1.Game.Play;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game
{
    public abstract class Wisdom : IUpdatable
    {
        

        private readonly GpiTransponder _Transponder;

        public GpiTransponder Transponder {  get { return _Transponder; } }

        Regulus.BehaviourTree.ITicker _Tree;

        private readonly Regulus.Utility.TimeCounter _DeltaTimeCounter;

        protected Wisdom()
        {
            
           
            _Transponder = new GpiTransponder();
            _DeltaTimeCounter = new TimeCounter();        
        }

        public ISoulBinder GetSoulBinder()
        {
            return _Transponder;
        }

        protected abstract Regulus.BehaviourTree.ITicker _Launch();
        protected abstract void _Update(float delta);
        protected abstract void _Shutdown();
        void IBootable.Launch()
        {
            _Tree = _Launch();
        }

        

        void IBootable.Shutdown()
        {
            _Shutdown();
        }

        bool IUpdatable.Update()
        {
            var second = _DeltaTimeCounter.Second;
            

            if (second > 0.03f)
            {                
                _Tree.Tick(second);
                _Update(second);
                _DeltaTimeCounter.Reset();                
            }

            return true;
        }
    }
}
