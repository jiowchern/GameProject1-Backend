using Regulus.BehaviourTree;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class WaitSecondStrategy : IAction
    {
        private readonly float _Second;

        private float _Count;
        public WaitSecondStrategy(float second)
        {
            _Second = second;            
        }

        TICKRESULT ITicker.Tick(float delta)
        {
            _Count += delta;
            if (_Count > _Second)
                return TICKRESULT.SUCCESS;

            return TICKRESULT.RUNNING;
        }

        void IAction.Start()
        {
            _Count = 0.0f;
        }

        void IAction.End()
        {            
        }
    }
}