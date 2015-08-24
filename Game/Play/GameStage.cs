using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

using Regulus.CustomType;
using Regulus.Game;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class GameStage : IStage , IController , IQuitable
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

        private List<IIndividual> _Controllers;

        

        public GameStage(ISoulBinder binder, GamePlayerRecord record, IGameRecorder recoder, Map map)
        {
            _Map = map;
            _Record = record;
            _Recoder = recoder;
            _Binder = binder;
            _SaveTimeCounter = new TimeCounter();
            _DeltaTimeCounter = new TimeCounter();
            _Controllers = new List<IIndividual>();
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
            return EntityProvider.Create("actor1", _Record.Name );
        }

        void IStage.Update()
        {
            _UpdateSave();

            var deltaTime = _GetDeltaTime();

            var velocity = _Status.GetVelocity(deltaTime);

            var orbit = _Mover.GetOrbit(velocity);
            var entitys = _Map.Find(orbit);
            _Mover.Move(velocity, entitys.Where(x => x.Id != _Player.Id));

        }

        private void _Broadcast(IEnumerable<IIndividual> controllers)
        {
            var current = _Controllers;

            _BroadcastJoin(controllers.Except(current));
            _BroadcastLeft(current.Except(controllers));

            _Controllers.Clear();
            _Controllers.AddRange(controllers);
        }

        private void _BroadcastLeft(IEnumerable<IIndividual> controllers)
        {
            foreach (var controller in controllers)
            {
                _Binder.Unbind<IVisible>(controller);
            }
        }

        private void _BroadcastJoin(IEnumerable<IIndividual> controllers)
        {
            foreach (var controller in controllers)
            {
                _Binder.Bind<IVisible>(controller);
            }
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

        void IController.Move(float angle)
        {
            _Status.Move(angle);
        }

        public void Quit()
        {
            DoneEvent.Invoke();
        }
    }
}