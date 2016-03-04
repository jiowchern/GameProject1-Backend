using System;
using System.Linq;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    [ProtoBuf.ProtoContract]
    public class Item
    {
       [ProtoBuf.ProtoMember(1)]
        public Guid Id { get; set; }
        
        [ProtoBuf.ProtoMember(2)]
        public string Name { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public int Weight { get; set; }
        [ProtoBuf.ProtoMember(4)]

        public Effect[] Effects { get; set; }

        [ProtoBuf.ProtoMember(5)]
        public int Count { get; set; }

        [ProtoBuf.ProtoMember(6)]
        public float Life { get; set; }

        public Item()
        {
            Effects = new Effect[0];
        }

        public Item Clone()
        {
            var newItem = new Item();
            newItem.Id = Guid.NewGuid();
            newItem.Count = Count;            
            newItem.Effects = Regulus.Utility.ValueHelper.DeepCopy(Effects);
            newItem.Name = Name;
            newItem.Weight = Weight;
            return newItem;
        }

        public static bool IsValid(Item src)
        {            

            return src != null && src.Id != Guid.Empty && src.Count > 0 && src.Effects != null;
        }

        public ItemPrototype GetPrototype()
        {
            var result = Resource.Instance.FindItem(Name);
            return result;
        }

        public bool IsEquipable()
        {
            var i = GetPrototype();
            if(i != null)
                return i.EquipPart != EQUIP_PART.NONE;

            return false;
        }

        public EQUIP_PART GetEquipPart()
        {
            var i = GetPrototype();
            if (i != null)
                return i.EquipPart  ;
            return EQUIP_PART.NONE;
        }

        public bool UpdateLife(float last_delta_time)
        {
            if (Life > 0)
            {
                Life -= last_delta_time;
            }
            return Life > 0;
        }

        public bool IsLife()
        {
            return Life > 0;
        }

        public float GetAid()
        {
            return (from e in Effects where e.Type == EFFECT_TYPE.AID select e.Value).Sum();
        }
    }
}