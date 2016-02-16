using System;
using System.Collections.Generic;

using Regulus.BehaviourTree;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class TurnHandler 
    {
        private readonly GoblinWisdom _GoblinWisdom;

        private float _Angle;

        private float _TimeCounter;

        private float _Delta;

        private readonly IEnumerator<TICKRESULT> _Iterator;

        public TurnHandler(GoblinWisdom goblin_wisdom )
        {
            _GoblinWisdom = goblin_wisdom;
            _Iterator = _Run().GetEnumerator();
        }

        public void Input(float angle)
        {
            _Angle = angle;
        }

        TICKRESULT _Tick(float delta)
        {
            _TimeCounter -= delta;
            if (_TimeCounter > 0)
            {                           
                return TICKRESULT.RUNNING;                    
            }
#if UNITY_EDITOR
            //UnityEngine.Debug.Log("Done TurnTimeCounter = " + _TimeCounter);
#endif
            _GoblinWisdom.StopTrun();
            return TICKRESULT.SUCCESS;
        }

        void _Start()
        {

            var turnSpeed = _GoblinWisdom.GetTrunSpeed();
            if (turnSpeed <= 0)
                return;
            var angle = _Angle;

            angle %= 360;
            angle += 360;
            angle %= 360;
            if(angle > 180)
                _TimeCounter = (180 - (angle % 180)) / turnSpeed;
            else
            {
                _TimeCounter = angle / turnSpeed;
            }


            if (angle > 180)
                _GoblinWisdom.MoveLeft();
            else if (angle <= 180)
                _GoblinWisdom.MoveRight();

#if UNITY_EDITOR
            // UnityEngine.Debug.Log("TurnTimeCounter = " + _TimeCounter);
            //UnityEngine.Debug.Log("Turn angle = " + angle);
#endif
        }

        void _End()
        {

            
        }

        public TICKRESULT Run(float delta)
        {
            _Delta = delta;
            _Iterator.MoveNext();
            var result = _Iterator.Current;
            return result;
        }

        IEnumerable<TICKRESULT> _Run()
        {
            while (true)
            {
                _Start();

                TICKRESULT result ;
                do
                {
                    result = _Tick(_Delta);
                    yield return result;
                }
                while (result == TICKRESULT.RUNNING);

                _End();
                yield return result;
            }
        }
    }
}