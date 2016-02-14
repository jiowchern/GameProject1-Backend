using System;
using System.Collections.Generic;


using System.Linq;


using Regulus.Collection;

using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;


namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class GameStage : IStage, IQuitable,
        IInventoryNotifier,
        
        IEquipmentNotifier
    {
        private readonly ISoulBinder _Binder;

        private readonly Map _Map;


        private readonly TimeCounter _DeltaTimeCounter;
        private readonly TimeCounter _UpdateTimeCounter;
        private const float _UpdateTime = 1.0f / 30.0f;

        private float _UpdateAllItemTime;

        private readonly Entity _Player;

        private readonly Mover _Mover;

        private readonly DifferenceNoticer<IIndividual> _DifferenceNoticer;



        private bool _RequestAllItems;

        private readonly ControlStatus _ControlStatus;

        private readonly Regulus.Utility.Updater _Updater;

        private readonly Wisdom _Wisdom;

        public GameStage(ISoulBinder binder, Map map, Entity entity)
        {
            _Map = map;
            _Binder = binder;
            _DeltaTimeCounter = new TimeCounter();
            _UpdateTimeCounter = new TimeCounter();
            _Updater = new Updater();
            _DifferenceNoticer = new DifferenceNoticer<IIndividual>();

            _Player = entity;
            _Mover = new Mover(this._Player);
            _ControlStatus = new ControlStatus(binder, _Player, _Mover, _Map);
        }
        public GameStage(ISoulBinder binder, Map map, Entity entity, Wisdom wisdom) : this(binder, map, entity)
        {
            _Wisdom = wisdom;


        }
        void IStage.Leave()
        {
            _Updater.Shutdown();
            _DifferenceNoticer.JoinEvent -= this._BroadcastJoin;
            _DifferenceNoticer.LeftEvent -= this._BroadcastLeft;

            _Binder.Unbind<IDevelopActor>(_Player);
            _Binder.Unbind<IInventoryNotifier>(this);
            _Binder.Unbind<IPlayerProperys>(_Player);
            _Binder.Unbind<IEquipmentNotifier>(this);
            _Map.Left(_Player);
        }

        void IStage.Enter()
        {
            this._DifferenceNoticer.JoinEvent += this._BroadcastJoin;
            this._DifferenceNoticer.LeftEvent += this._BroadcastLeft;

            this._Map.JoinChallenger(this._Player);
            this._Binder.Bind<IPlayerProperys>(_Player);
            this._Binder.Bind<IInventoryNotifier>(this);
            this._Binder.Bind<IEquipmentNotifier>(this);
            _Binder.Bind<IDevelopActor>(_Player);
            _Updater.Add(_ControlStatus);

            if (_Wisdom != null)
                _Updater.Add(_Wisdom);
        }



        void IStage.Update()
        {
            if (_UpdateTimeCounter.Second < _UpdateTime)
                return;
            var deltaTime = this._GetDeltaTime();
            _Updater.Working();

            var lastDeltaTime = deltaTime;
            _Move(lastDeltaTime);
            _Broadcast(_Map.Find(_Player.GetView()));
            _Player.Equipment.UpdateEffect(lastDeltaTime);

            _ResponseItems(deltaTime);


            _Player.Strength(deltaTime);

        }




        private void _ResponseItems(float deltaTime)
        {
            if (_UpdateAllItemTime - deltaTime <= 0)
            {
                if (_RequestAllItems)
                {
                    _FlushEvent(_Player.Equipment.GetAll());
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


        private void _Move(float deltaTime)
        {
            _Player.ClearCollisionTargets();
            _Player.TrunDirection(deltaTime);
            var velocity = this._Player.GetVelocity(deltaTime);
            var orbit = this._Mover.GetOrbit(velocity);
            var entitys = this._Map.Find(orbit);
            var hitthetargets = _Mover.Move(velocity, entitys.Where(x => x.Id != this._Player.Id));
            if (hitthetargets.Any())
            {
                _Player.SetCollisionTargets(hitthetargets);
                this._Player.ClearOffset();
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





        public event Action DoneEvent;
        



        public void Quit()
        {
            this.DoneEvent.Invoke();
        }

        private event Action<Item[]> _FlushEvent;
        event Action<Item[]> IEquipmentNotifier.FlushEvent
        {
            add { _FlushEvent += value; }
            remove { _FlushEvent += value; }
        }

        void IEquipmentNotifier.Query()
        {
            _RequestAllItems = true;
        }

        void IEquipmentNotifier.Unequip(Guid id)
        {
            var item = _Player.Equipment.Unequip(id);
            if (item.IsValid())
            {
                _Player.Bag.Add(item);
            }
        }

        void IInventoryNotifier.Query()
        {
            _RequestAllItems = true;
        }

        void IInventoryNotifier.Discard(Guid id)
        {
            _Player.Bag.Remove(id);
        }

        void IInventoryNotifier.Equip(Guid id)
        {
            var item = _Player.Bag.Find(id);
            if (item.IsValid() && item.IsEquipable())
            {

                var equipItem = _Player.Equipment.Unequip(item.GetEquipPart());
                if (equipItem.IsValid())
                {
                    _Player.Bag.Add(equipItem);
                }
                _Player.Bag.Remove(item.Id);

                _Player.Equipment.Equip(item);
            }
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


        event Action<Guid> IEquipmentNotifier.RemoveEvent
        {
            add { _Player.Equipment.RemoveEvent += value; }
            remove { _Player.Equipment.RemoveEvent -= value; }
        }


        event Action<Item> IEquipmentNotifier.AddEvent
        {
            add { _Player.Equipment.AddEvent += value; }
            remove { _Player.Equipment.AddEvent -= value; }
        }

        event Action<Guid> IInventoryNotifier.RemoveEvent
        {
            add { _Player.Bag.RemoveEvent += value; }
            remove { _Player.Bag.RemoveEvent -= value; }
        }


    }
}