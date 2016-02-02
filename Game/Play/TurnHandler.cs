using System;

using Regulus.BehaviourTree;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class TurnHandler : IAction
    {
        private readonly GoblinWisdom _GoblinWisdom;

        

        private float _TimeCounter;
        public TurnHandler(GoblinWisdom goblin_wisdom)
        {
            _GoblinWisdom = goblin_wisdom;
            
        }

        TICKRESULT ITicker.Tick(float delta)
        {
            if (_TimeCounter > 0)
            {
                _TimeCounter -= delta;
            
                return TICKRESULT.RUNNING;
                    
            }
            
            return TICKRESULT.SUCCESS;
        }

        void IAction.Start()
        {
            
            var angle = _GoblinWisdom.TurnDirection ;            

            _TimeCounter = Math.Abs(angle) / 300f;

            if (angle < 0)
                _GoblinWisdom.MoveLeft();
            else
                _GoblinWisdom.MoveRight();
        }

        void IAction.End()
        {
            
            _GoblinWisdom.StopTrun();
        }
    }
}