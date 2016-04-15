using System;

using Regulus.BehaviourTree;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class ChestIdleAction : IAction
    {
        private readonly Entity _Chest;

        private readonly IMapGate _Gate;


        private TICKRESULT _Status;

        public ChestIdleAction(Entity chest, IMapGate gate)
        {
            _Chest = chest;
            _Gate = gate;            
        }

        void ITicker.Reset()
        {
            
        }

        TICKRESULT ITicker.Tick(float delta)
        {
            return _Status;
        }

        void IAction.Start()
        {
            _Chest.UnlockEvent += _OnUnlcokResult;
            _Gate.Join(_Chest);
            _Status = TICKRESULT.RUNNING;
        }

        

        void IAction.End()
        {
            _Gate.Left(_Chest);
            _Chest.UnlockEvent -= _OnUnlcokResult;
        }

        private void _OnUnlcokResult(bool result)
        {
            if (result == false)
            {
                _Status = TICKRESULT.SUCCESS;
            }
                
        }
    }
}