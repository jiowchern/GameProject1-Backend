using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    abstract internal class ActorStatus : IStage
    {        

        public abstract void Enter();

        public abstract void Leave();

        public abstract void Update();
    }
}