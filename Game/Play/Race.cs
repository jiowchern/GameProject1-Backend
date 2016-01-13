using System.Collections.Generic;

using Regulus.Framework;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class Race :Regulus.Utility.IUpdatable
    {
        private readonly Zone _Zone;

        private readonly List<Aboriginal> _Aboriginals;

        private readonly Regulus.Utility.Updater _Updater;
        public Race(Zone zone)
        {
            _Aboriginals = new List<Aboriginal>();
            this._Zone = zone;
            _Updater = new Updater();
            for (int i = 0; i < 200; i++)
            {

                _Updater.Add(new Aboriginal(_Zone));
            }
            
            
        }

        void IBootable.Launch()
        {
            
        }

        void IBootable.Shutdown()
        {
            _Updater.Shutdown();
        }

        bool IUpdatable.Update()
        {
            _Updater.Working();
            return true;
        }
    }
}