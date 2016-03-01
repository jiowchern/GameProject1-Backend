using System;
using System.Linq;
using System.Collections.Generic;

using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class InventoryProxy 
    {
        private IInventoryNotifier _Notifier;

        private List<Item> _Items;

        public InventoryProxy()
        {
            _Items = new List<Item>();
        }
        public void Clear()
        {
            _Items.Clear();
            if (_Notifier != null)
            {
                _Notifier.AddEvent -= _Add;
                _Notifier.AllItemEvent -= _Flush;
                _Notifier.RemoveEvent -= _Remove;
                _Notifier = null;
            }
            
        }

        public void Set(IInventoryNotifier inventory_notifier)
        {
            Clear();
            _Notifier = inventory_notifier;
            _Notifier.AllItemEvent += _Flush;
            _Notifier.AddEvent += _Add;
            _Notifier.RemoveEvent += _Remove;
            _Notifier.Query();
        }

        private void _Remove(Guid obj)
        {
            _Items.RemoveAll(i => i.Id == obj);
        }

        private void _Add(Item obj)
        {
            _Items.Add(obj);
        }

        private void _Flush(Item[] obj)
        {
            _Items.Clear();
            _Items.AddRange(obj);
        }

        public IEnumerable<Item> FindByPart(EQUIP_PART part)
        {
            return from item in _Items where item.GetEquipPart() == part select item;
        }

        public void Equip(Guid id)
        {
            _Notifier.Equip(id);
        }
    }
}