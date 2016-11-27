using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class ItemProvider
    {
        public Item[] FromStolen()
        {
            return (from i in Resource.Instance.Items
                   let winning = Regulus.Utility.Random.Instance.NextFloat(0.0f, 1.0f) > 0.5f
                   where winning && i.Features == ITEM_FEATURES.MATERIAL
                   select CreateItem(i.Id , 10) ).ToArray();
        }

        public Item BuildItem(float quality, Item item, ItemEffect[] item_effects)
        {
            var effectss = from e in item_effects
                           where quality >= e.Quality
                           select e.Effects;
            List<Effect> effects = new List<Effect>();
            foreach (var effectse in effectss)
            {
                effects.AddRange(effectse);
            }
            var groupEffects = from e in effects group e by e.Type;
            var resultEffect = from g in groupEffects
                               select new Effect()
                               {
                                   Type = g.Key,
                                   Value = g.Sum( q=> q.Value )
                               };
            item.Effects = resultEffect.ToArray();

            item.Life = _GetLife(item.Effects);
            return item;
        }

        private float _GetLife(Effect[] effects)
        {
            return (from e in effects where e.Type == EFFECT_TYPE.LIFE select e.Value).Sum();
        }

        public Item BuildItem(float quality,string name , ItemEffect[] item_effects)
        {
            var item = CreateItem(name);
            return BuildItem(quality , item , item_effects);
        }

        public Item CreateItem(string name , int count)
        {
            return (from i in Resource.Instance.Items
                    where i.Id == name
                    select new Item() { Id = Guid.NewGuid(), Name = name, Weight = 10, Effects = new Effect[0], Count = count }).Single();
        }
        public Item CreateItem(string name)
        {
            return CreateItem(name , 1);
        }
    }
}