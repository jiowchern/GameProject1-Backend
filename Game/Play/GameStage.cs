using System;
using System.Collections.Generic;


using System.Linq;


using Regulus.Collection;

using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;


namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class GameStage : IStage , IQuitable, IInventoryNotifier ,IPlayerProperys
    {
        private readonly GamePlayerRecord _Record;
        private readonly IGameRecorder _Recoder;
        private readonly ISoulBinder _Binder;

        private readonly Map _Map;

        private readonly TimeCounter _SaveTimeCounter;
        private readonly TimeCounter _DeltaTimeCounter;
        private const float _UpdateTime = 1.0f / 30.0f;

        private float _UpdateAllItemTime;

        private readonly Entity _Player;

        private readonly Mover _Mover;
        
        private readonly DifferenceNoticer<IIndividual> _DifferenceNoticer;

        float _UpdateTimeCount;

        private bool _RequestAllItems;

        private readonly ControlStatus _ControlStatus;

        private readonly Regulus.Utility.Updater _Updater;

        

        public GameStage(ISoulBinder binder, GamePlayerRecord record, IGameRecorder recoder, Map map)
        {
            _Map = map;
            _Record = record;
            _Recoder = recoder;
            _Binder = binder;
            _SaveTimeCounter = new TimeCounter();
            _DeltaTimeCounter = new TimeCounter();
            _Updater = new Updater();
            _DifferenceNoticer = new DifferenceNoticer<IIndividual>();

            _Player = this._CreatePlayer();
            _Mover = new Mover(this._Player);
            _ControlStatus = new ControlStatus(binder, _Player, _Mover , _Map);

        }
        void IStage.Leave()
        {
            _Updater.Shutdown();
            _DifferenceNoticer.JoinEvent -= this._BroadcastJoin;
            _DifferenceNoticer.LeftEvent -= this._BroadcastLeft;

            _Binder.Unbind<IInventoryNotifier>(this);
            _Binder.Unbind<IPlayerProperys>(this);
            _Map.Left(_Player);
            _Save();
        }        

        void IStage.Enter()
        {
            this._DifferenceNoticer.JoinEvent += this._BroadcastJoin;
            this._DifferenceNoticer.LeftEvent += this._BroadcastLeft;

            this._Map.JoinChallenger(this._Player);            
            this._Binder.Bind<IPlayerProperys>(this);
            this._Binder.Bind<IInventoryNotifier>(this);

            _Updater.Add(_ControlStatus);
        }

        private Entity _CreatePlayer()
        {
            var player = EntityProvider.Create(this._Record);
                        
            foreach (var item in _Record.Items)                
            {
                player.Bag.Add(item);
            }
            return player;
        }

        void IStage.Update()
        {
            _Updater.Working();
            _UpdateSave();
            
            var deltaTime = this._GetDeltaTime();
            if (_TimeUp(deltaTime))
            {                
                _Move(deltaTime + GameStage._UpdateTime);
                _Broadcast(_Map.Find(_Player.GetView()));
            }
            
            _ResponseItems(deltaTime);

        }
        

        

        private void _ResponseItems(float deltaTime)
        {
            if (_UpdateAllItemTime - deltaTime <= 0)
            {
                if (_RequestAllItems)
                {
                    _AllItemEvent.Invoke(_Player.Bag.GetAll());
                    _UpdateAllItemTime = 10f;
                    _RequestAllItems = false;
                }
            }
            else
            {
                _UpdateAllItemTime -= deltaTime;
            }
        }

        private bool _TimeUp(float deltaTime)
        {
            this._UpdateTimeCount += deltaTime;
            if(this._UpdateTimeCount >= GameStage._UpdateTime)
            {
                this._UpdateTimeCount %= GameStage._UpdateTime;
                return true;
            }
            return false;
        }

        private void _Move(float deltaTime)
        {
            var velocity = this._Player.GetVelocity(deltaTime);
            var orbit = this._Mover.GetOrbit(velocity);
            var entitys = this._Map.Find(orbit);
            if(this._Mover.Move(velocity, entitys.Where(x => x.Id != this._Player.Id)) == false)
            {
                this._Player.Stop();
            }
        }

        private void _Broadcast(IEnumerable<IIndividual> controllers)
        {
            this._DifferenceNoticer.Set(controllers);
        }

        private void _BroadcastLeft(IEnumerable<IIndividual> controllers)
        {
            foreach (var controller in controllers)
            {
                this._Binder.Unbind<IVisible>(controller);
            }
        }

        private void _BroadcastJoin(IEnumerable<IIndividual> controllers)
        {
            foreach (var controller in controllers)
            {
                this._Binder.Bind<IVisible>(controller);
            }
        }

        private float _GetDeltaTime()
        {
            var second = this._DeltaTimeCounter.Second;
            this._DeltaTimeCounter.Reset();
            return second;
        }
        

        private void _UpdateSave()
        {
            if (this._SaveTimeCounter.Second >= 60)
            {
                this._Save();
                this._SaveTimeCounter.Reset();
            }
        }

        private void _Save()
        {
            this._Recoder.Save(this._Record);
        }

        public event Action DoneEvent;

        Guid IPlayerProperys.Id { get { return this._Player.Id; } }

       

        public void Quit()
        {
            this.DoneEvent.Invoke();
        }

        void IInventoryNotifier.Query()
        {
            _RequestAllItems = true;
        }

        void IInventoryNotifier.Discard(Guid id)
        {
            _Player.Bag.Remove(id);
        }

        private event Action<Item[]> _AllItemEvent;
        event Action<Item[]> IInventoryNotifier.AllItemEvent
        {
            add { _AllItemEvent += value; }
            remove { _AllItemEvent -= value; }
        }

        event Action<Item> IInventoryNotifier.AddEvent
        {
            add { _Player.Bag.AddEvent += value; }
            remove { _Player.Bag.AddEvent -= value; }
        }

        event Action<Guid> IInventoryNotifier.RemoveEvent
        {
            add { _Player.Bag.RemoveEvent += value; }
            remove { _Player.Bag.RemoveEvent -= value; }
        }

        
    }
}