using Regulus.Framework;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class AboriginalHall :Regulus.Utility.IUpdatable
    {
        private readonly Zone _Zone;

        public AboriginalHall(Zone zone)
        {
            this._Zone = zone;            
        }

        void IBootable.Launch()
        {
            
        }

        void IBootable.Shutdown()
        {
            throw new System.NotImplementedException();
        }

        bool IUpdatable.Update()
        {
            throw new System.NotImplementedException();
        }
    }
}