using Regulus.BehaviourTree;
using Regulus.Framework;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class PortalWisdom : Wisdom 
    {
        private readonly Entity _Entity;

        private readonly string _TargetRealm;

        private readonly IMapGate _Gate;

        private readonly IMapFinder _Finder;

        public PortalWisdom(Entity entity, string target_realm , IMapGate gate, IMapFinder finder)
        {
            _Entity = entity;
            _TargetRealm = target_realm;
            _Gate = gate;
            _Finder = finder;
        }

        protected override ITicker _Launch()
        {
            _Gate.Join(_Entity);


            var builder = new Regulus.BehaviourTree.Builder();
            var ticker = builder
                    .Sequence()
                        .Action(()=>new WaitSecondStrategy(0.1f) )
                        .Action(_HandlePassing)
                    .End()
                .Build();
            return ticker;
        }

        protected override void _Update(float delta)
        {
            
        }

        private TICKRESULT _HandlePassing(float delta)
        {
            var targets = _Finder.Find(_Entity.GetBound());
            foreach (var individual in targets)
            {
                if(individual.EntityType == ENTITY.ACTOR1)
                    individual.Transmit(_TargetRealm);
            }
            return TICKRESULT.SUCCESS;
        }

        protected override void _Shutdown()
        {
            _Gate.Left(_Entity);
        }

       
    }
}