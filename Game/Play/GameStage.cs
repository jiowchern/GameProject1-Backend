using System;
using System.Globalization;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class GameStage : IStage
    {
        private readonly GamePlayerRecord _Record;
        private readonly IGameRecorder _Recoder;
        private readonly ISoulBinder _Binder;

        private IMap _Map;

        private Regulus.Utility.TimeCounter _TimeCounter;
        public GameStage(ISoulBinder binder, GamePlayerRecord record, IGameRecorder recoder, IMap map)
        {
            _Map = map;
            _Record = record;
            _Recoder = recoder;
            _Binder = binder;
            _TimeCounter = new TimeCounter();
        }
        void IStage.Leave()
        {
            
            var player = _CreatePlayer();
            _Map.ChangeEvent -= _ChangeLocation;
            _Map.Left(player);
            _Save();
        }        

        void IStage.Enter()
        {

            var player = _CreatePlayer();
            _Map.Join(player, _Record.Start );
            _Map.ChangeEvent += _ChangeLocation;
        }

        private IVisible _CreatePlayer()
        {
            throw new NotImplementedException();
        }

        void IStage.Update()
        {
            _UpdateSave();

            
        }
        private void _ChangeLocation(IVisible arg1, Location arg2)
        {
            throw new NotImplementedException();
        }

        private void _UpdateSave()
        {
            if (_TimeCounter.Second >= 60)
            {
                _Save();
                _TimeCounter.Reset();
            }
        }

        private void _Save()
        {
            _Recoder.Save(_Record);
        }

        public event Action DoneEvent;
    }
}