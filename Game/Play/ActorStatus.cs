using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    abstract internal class ActorStatus : IStage
    {
        public readonly  ACTOR_STATUS_TYPE ActorStatusType;

        

        protected ActorStatus(ACTOR_STATUS_TYPE actor_status_type)
        {
            ActorStatusType = actor_status_type;            
        }

        public abstract void Enter();

        public abstract void Leave();

        public abstract void Update();
    }
}