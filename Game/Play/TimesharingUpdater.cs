using System.Linq;

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
}