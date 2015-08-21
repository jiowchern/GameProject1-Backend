using System;
using System.Globalization;
using System.Linq;

using Regulus.CustomType;
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

        private Map _Map;

        private Regulus.Utility.TimeCounter _SaveTimeCounter;
        private Regulus.Utility.TimeCounter _DeltaTimeCounter;

        private Entity _Player;

        private EntityStatus _Status;

        private Mover _Mover;

        

        public GameStage(ISoulBinder binder, GamePlayerRecord record, IGameRecorder recoder, Map map)
        {
            _Map = map;
            _Record = record;
            _Recoder = recoder;
            _Binder = binder;
            _SaveTimeCounter = new TimeCounter();
            _DeltaTimeCounter = new TimeCounter();
        }
        void IStage.Leave()
        {
            
            _Map.Left(_Player);
            _Save();
        }        

        void IStage.Enter()
        {
            _Player = _CreatePlayer();
            _Map.Join(_Player);
            
        }

        private Entity _CreatePlayer()
        {
            var mesh = new Polygon(new []
            {
                new Vector2(0,0),
                new Vector2(0,1),
                new Vector2(1,1),
                new Vector2(1,0),
            });
            
            return new Entity(mesh);
        }

        void IStage.Update()
        {
            _UpdateSave();

            var deltaTime = _GetDeltaTime();

            var velocity = _Status.GetSpeed(deltaTime);

            var orbit = _Mover.GetOrbit(velocity);
            var entitys = _Map.Find(orbit).Where(x => x.Id != _Player.Id );
            _Mover.Move(velocity, entitys); 


        }
        

        private float _GetDeltaTime()
        {
            var second = _DeltaTimeCounter.Second;
            _DeltaTimeCounter.Reset();
            return second;
        }
        

        private void _UpdateSave()
        {
            if (this._SaveTimeCounter.Second >= 60)
            {
                _Save();
                this._SaveTimeCounter.Reset();
            }
        }

        private void _Save()
        {
            _Recoder.Save(_Record);
        }

        public event Action DoneEvent;
    }
}