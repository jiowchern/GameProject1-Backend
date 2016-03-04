using System;

using Regulus.BehaviourTree;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class TraceStrategy
    {        

        private readonly GoblinWisdom _Wisdom;
        private Regulus.BehaviourTree.ITicker _Node;

        public TraceStrategy(GoblinWisdom wisdom)
        {
            _Wisdom = wisdom;
        }
        public TICKRESULT Set(Guid target, float distance, float angle_f)
        {
            var th = new TurnHandler(_Wisdom);
            
            float angle=0.0f;
            var builder = new Regulus.BehaviourTree.Builder();            
            _Node = builder.Sequence()
                        .Action(
                           (delta) =>
                           {
                               var result = _Wisdom.GetTargetAngle(target, ref angle);
                               th.Input(angle);
                               return result;
                           })
                        .Action((delta) => th.Run(delta))

                        .Not()
                            .Sequence()
                                .Action((delta) => _Not(_Wisdom.CheckDistance(target, distance)))
                                .Action(_Wisdom.MoveForward)
                            .End()
                        .End()                        
                        .Action((delta) => _Wisdom.GetTargetAngle(target, ref angle))
                        .Action((delta) => _Wisdom.CheckAngle(angle))
                        .Action(_Wisdom.StopMove )
                   .End().Build();


            return TICKRESULT.SUCCESS;
        }

        public TICKRESULT Tick(float delta)
        {
            return _Node.Tick(delta);
        }


        private TICKRESULT _Not(TICKRESULT check)
        {
            if (check == TICKRESULT.FAILURE)
                return TICKRESULT.SUCCESS;
            if (check == TICKRESULT.SUCCESS)
                return TICKRESULT.FAILURE;
            return TICKRESULT.RUNNING;
        }
    }
}