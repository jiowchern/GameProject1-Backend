using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

using Regulus.Collection;
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

        private readonly Map _Map;

        private readonly Regulus.Utility.TimeCounter _SaveTimeCounter;
        private readonly Regulus.Utility.TimeCounter _DeltaTimeCounter;

        private readonly Entity _Player;

        

        private readonly Mover _Mover;

        private readonly Regulus.Collection.DifferenceNoticer<IIndividual> _DifferenceNoticer;




        public GameStage(ISoulBinder binder, GamePlayerRecord record, IGameRecorder recoder, Map map)
        {
            _Map = map;            
            _Record = record;
            _Recoder = recoder;
            _Binder = binder;
            _SaveTimeCounter = new TimeCounter();
            _DeltaTimeCounter = new TimeCounter();
            _DifferenceNoticer = new DifferenceNoticer<IIndividual>();

            _Player = _CreatePlayer();
            _Mover = new Mover(_Player);        


        }
        void IStage.Leave()
        {
            _DifferenceNoticer.JoinEvent -= _BroadcastJoin;
            _DifferenceNoticer.LeftEvent -= _BroadcastLeft;

            _Binder.Unbind<IController>(this);
            _Map.Left(_Player);
            _Save();
        }        

        void IStage.Enter()
        {
            _DifferenceNoticer.JoinEvent += _BroadcastJoin;
            _DifferenceNoticer.LeftEvent += _BroadcastLeft;

            _Map.JoinChallenger(_Player);
            _Binder.Bind<IController>(this);
        }

        private Entity _CreatePlayer()
        {
            return EntityProvider.Create(_Record );
        }

        void IStage.Update()
        {
            _UpdateSave();

            var deltaTime = _GetDeltaTime();

            var velocity = _Player.GetVelocity(deltaTime);

            var orbit = _Mover.GetOrbit(velocity);
            var entitys = _Map.Find(orbit);
            if (_Mover.Move(velocity, entitys.Where(x => x.Id != _Player.Id)) == false)
            {
                _Player.Stop();
            }

            _Broadcast(_Map.Find(_Player));
        }

        private void _Broadcast(IEnumerable<IIndividual> controllers)
        {
            _DifferenceNoticer.Set(controllers);
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
            if (_SaveTimeCounter.Second >= 60)
            {
                _Save();
                _SaveTimeCounter.Reset();
            }
        }

        private void _Save()
        {
            _Recoder.Save(_Record);
        }

        public event Action DoneEvent;

        Guid IController.Id { get { return _Player.Id; } }

        void IController.Move(float angle)
        {
            _Player.Move(angle);
        }

        void IController.Stop()
        {
            _Player.Stop();
        }

        public void Quit()
        {
            DoneEvent.Invoke();
        }
    }
}