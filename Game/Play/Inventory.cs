using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Regulus.Game.Data;
using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Inventory 
    {
        private readonly List<Item> _Items;

        private int _Weight;


        public Inventory()
        {
            this._Items = new List<Item>();
        }
        public void Add(Item item)
        {
            if (item.Effects.Length == 0)
            {
                var inBagItem = (from i in _Items where i.Name == item.Name && i.Effects.Length == 0 select i).FirstOrDefault();
                if(Item.IsValid(inBagItem))
                {
                    item.Count += inBagItem.Count;
                    Remove(inBagItem.Id);
                }                
            }
            this._Items.Add(item);
            this._Weight += item.Weight;

            if (AddEvent != null)
                AddEvent(item);
        }

        public int GetWeight()
        {
            return this._Weight;
        }

        public void Remove(Guid id)
        {
            int weight = 0;
            this._Items.RemoveAll(
                (item) =>
                {
                    if (item.Id == id)
                    {
                        weight+=item.Weight;
                        if (RemoveEvent != null)
                        {
                            RemoveEvent(id);
                        }
                        return true;        
                    }
                    return false;
                });

            this._Weight -= weight;
        }

        public Item[] GetAll()
        {
            return _Items.ToArray();
        }

        public event Action<Item> AddEvent;

        public event Action<Guid> RemoveEvent;

        public int GetItemAmount(string item)
        {

            return (from i in _Items
                    where i.Name == item
                    select i.Count).Sum();
        }

        public int Remove(string name, int amount)
        {
            List<Guid> removeItems = new List<Guid>();
            Item newItem = new Item();
            foreach(var item in _Items)
            {
                if(item.Name != name)
                    continue;

                amount -= item.Count;
                removeItems.Add(item.Id);

                if (amount == 0)
                    break;
                
                if (amount < 0)
                {
                    newItem = item.Clone();
                    newItem.Count =- amount;
                    break;
                }                
            }
            

            foreach(var item in removeItems)
            {
                Remove(item);
            }

            if (Item.IsValid(newItem))
            {
                Add(newItem);
            }
            return amount ;
        }

        public Item Find(Guid id)
        {
            return _Items.FirstOrDefault(i => i.Id == id);
        }
    }
}
