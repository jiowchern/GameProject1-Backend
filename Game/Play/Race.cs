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

        private TimeCounter _Counter;

        public TimesharingUpdater(float timeup_per_loop)
        {
            _TimeupPerLoop = timeup_per_loop;
            _Counter = new Regulus.Utility.TimeCounter();
        }

        public void Working()
        {
            
            var count = 0;
            var second = 0f;
            var array = _GetObjectSet().ToArray();
            _Counter.Reset();
            while (second <= _TimeupPerLoop && count < array.Length)
            {
                var updater = array[_Index];
                count++;
                _Index++;
                updater.Update();

                if (_Index == array.Length)
                {
                    _Index = 0;
                }                    
                                
                second = _Counter.Second;
            }
            
            
        }
    }

    internal class Race :Regulus.Utility.IUpdatable
    {
        private readonly Zone _Zone;

        

        private readonly TimesharingUpdater _Updater;
        public Race(Zone zone)
        {
            var itemProvider = new ItemProvider();
            this._Zone = zone;
            _Updater = new TimesharingUpdater(1.0f / 30.0f);

            for (int i = 0; i < 50; i++)
            {                
                var entiry = _Create(ENTITY.ACTOR2,  itemProvider);
                _Updater.Add(entiry);                
            }

            for (int i = 0; i < 50; i++)
            {
                var entiry = _Create(ENTITY.ACTOR3, itemProvider);
                _Updater.Add(entiry);
            }

            for (int i = 0; i < 50; i++)
            {
                var entiry = _Create(ENTITY.ACTOR4, itemProvider);
                _Updater.Add(entiry);
            }

            for (int i = 0; i < 50; i++)
            {
                var entiry = _Create(ENTITY.ACTOR5, itemProvider);
                _Updater.Add(entiry);
            }
            

        }

        private Aboriginal _Create(ENTITY type, ItemProvider itemProvider)
        {
            var entiry = EntityProvider.Create(type);
            var items = itemProvider.FromStolen();
            foreach (var item in items)
            {
                entiry.Bag.Add(item);
            }

            var wisdom = new StandardWisdom(entiry);
            return new Aboriginal(_Zone, entiry, wisdom);
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