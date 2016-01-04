using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Inventory 
    {
        private List<Item> _Items;

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
                item.Count += inBagItem.Count;
                Remove(inBagItem.Id);
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
    }
}
