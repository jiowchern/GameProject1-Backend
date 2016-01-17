using System;
using System.Collections.Generic;
using System.Linq;

using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Equipment
    {
        private readonly Entity _Entity;

        private readonly Dictionary<EQUIP_PART, Item> _Items;

        

        public event Action<Item> AddEvent;
        public event Action<Guid> RemoveEvent;
        public Equipment(Entity entity)
        {
            this._Entity = entity;
            this._Items = new Dictionary<EQUIP_PART, Item>();
        }

        public Item Unequip(EQUIP_PART equip_type)
        {
            Item item;
            _Items.TryGetValue(equip_type, out item);
            if(item.IsValid())
            {
                _Items.Remove(equip_type);
                RemoveEvent(item.Id);
            }                
            return item;
        }

        public void Equip(Item item)
        {
            if(_Items.ContainsKey(item.GetEquipPart()) == false)
            {
                _Items.Add(item.GetEquipPart() , item);
                AddEvent(item);
            }            
        }


        public Item? Find(EQUIP_PART part)
        {
            if (_Items.ContainsKey(part))
                return _Items[part];

            return null;
        }
        public Item[] GetAll()
        {
            return _Items.Values.ToArray();
        }

        public Item Unequip(Guid id)
        {
            var part = (from item in _Items.Values where  item.Id == id
            select item.GetEquipPart()).FirstOrDefault();
            return Unequip(part);
        }

        public void UpdateEffect(float last_delta_time)
        {
            foreach(var item in _Items)
            {
                
            }
        }
    }
}