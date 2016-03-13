using System;
using System.Linq;

using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class NormalStatus : ActorStatus , INormalSkill , IInventoryNotifier , IEquipmentNotifier
    {
        private readonly ISoulBinder _Binder;

        private readonly Entity _Player;

        public event Action<Guid> ExploreEvent;

        public event Action<Guid> AidEvent;

        public event Action<SkillCaster> BattleEvent;
        public event Action MakeEvent;

        private readonly MoveController _MoveController;

        private bool _RequestAllItems;

        private Regulus.Utility.TimeCounter _TimeCounter;

        private float _UpdateAllItemTime;

        public NormalStatus(ISoulBinder binder, Entity player) 
        {
            _TimeCounter = new TimeCounter();
            _Binder = binder;
            _Player = player;

            _MoveController = new MoveController(_Player);
        }
        

        public override void Leave()
        {
            _Binder.Unbind<INormalSkill>(this);
            _Binder.Unbind<IMoveController>(_MoveController);
            _Binder.Unbind<IInventoryNotifier>(this);
            _Binder.Unbind<IEquipmentNotifier>(this);
        }

        public override void Update()
        {
            _ResponseItems(_TimeCounter.Second);
            _TimeCounter.Reset();
        }

        public override void Enter()
        {
            _Binder.Bind<IEquipmentNotifier>(this);
            this._Binder.Bind<IInventoryNotifier>(this);
            _Binder.Bind<IMoveController>(_MoveController);
            _Binder.Bind<INormalSkill>(this);

            _Player.Normal();
        }

        private void _ResponseItems(float deltaTime)
        {
            if (_UpdateAllItemTime - deltaTime <= 0)
            {
                if (_RequestAllItems)
                {
                    if (_FlushEvent != null)
                    {
                        _FlushEvent(_Player.Equipment.GetAll());
                    }
                    if (_AllItemEvent != null)
                    {
                        _AllItemEvent.Invoke(_Player.Bag.ToArray());
                    }
                    _UpdateAllItemTime = 10f;
                    _RequestAllItems = false;
                }
            }
            else
            {
                _UpdateAllItemTime -= deltaTime;
            }
        }

        
        

        

        void INormalSkill.Explore(Guid target)
        {
            ExploreEvent(target);
        }

        public void Battle()
        {
            BattleEvent(_Player.GetBattleCaster());
        }

        void INormalSkill.Make()
        {
            MakeEvent();
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
            if (Item.IsValid(item))
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
            if (Item.IsValid(item) && item.IsEquipable())
            {

                var equipItem = _Player.Equipment.Unequip(item.GetEquipPart());
                if (Item.IsValid(equipItem))
                {
                    _Player.Bag.Add(equipItem);
                }
                _Player.Bag.Remove(item.Id);

                _Player.Equipment.Equip(item);
            }
        }

        void IInventoryNotifier.Use(Guid id)
        {
            var item = _Player.Bag.Find(id);
            if (item != null)
            {
                if (item.GetAid() > 0)
                    AidEvent(id);
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