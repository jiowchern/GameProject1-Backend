using System.Collections.Generic;
using System.Linq;

using Regulus.Framework;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{

    class TimesharingUpdater : Launcher<IUpdatable>
    {
        private readonly float _TimeupPerLoop;

        private int _Index;
        public TimesharingUpdater(float timeup_per_loop)
        {
            _TimeupPerLoop = timeup_per_loop;
            
        }

        public void Working()
        {
            var counter = new Regulus.Utility.TimeCounter();
            while (counter.Second <= _TimeupPerLoop)
            {
                var updater = _GetObjectSet().Skip(_Index).FirstOrDefault();
                if (updater != null)
                {
                    _Index++;
                    updater.Update();
                }                    
                else
                {
                    _Index = 0;
                    break;                   
                }
            }
        }
    }

    internal class Race :Regulus.Utility.IUpdatable
    {
        private readonly Zone _Zone;

        private readonly List<Aboriginal> _Aboriginals;

        private readonly TimesharingUpdater _Updater;
        public Race(Zone zone)
        {
            _Aboriginals = new List<Aboriginal>();
            this._Zone = zone;
            _Updater = new TimesharingUpdater(1.0f / 60.0f);

            for (int i = 0; i < 50; i++)
            {
                var entiry = EntityProvider.Create(ENTITY.ACTOR2);
                var wisdom = new UnityChanWisdom(entiry);
                _Updater.Add(new Aboriginal(_Zone , entiry , wisdom));
            }

            for (int i = 0; i < 50; i++)
            {
                var entiry = EntityProvider.Create(ENTITY.ACTOR3);
                var wisdom = new GoblinWisdom(entiry);
                _Updater.Add(new Aboriginal(_Zone, entiry, wisdom));
            }

            for (int i = 0; i < 50; i++)
            {
                var entiry = EntityProvider.Create(ENTITY.ACTOR4);
                var wisdom = new GoblinWisdom(entiry);
                _Updater.Add(new Aboriginal(_Zone, entiry, wisdom));
            }

            for (int i = 0; i < 50; i++)
            {
                var entiry = EntityProvider.Create(ENTITY.ACTOR5);
                var wisdom = new GoblinWisdom(entiry);
                _Updater.Add(new Aboriginal(_Zone, entiry, wisdom));
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