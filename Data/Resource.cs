using System;
using System.Linq;
namespace Regulus.Project.ItIsNotAGame1.Data
{
    public class Resource : Utility.Singleton<Resource>
    {
        public TownData[] Towns;
        public EntityData[] Entitys;
        public SkillData[] SkillDatas;

        public ItemPrototype[] Items;

        public ItemFormula[] Formulas;

        public EntityGroupLayout[] EntityGroupLayouts;

        public Resource()
        {
            Towns = new TownData[0];
            EntityGroupLayouts = new EntityGroupLayout[0];
            Entitys = new EntityData[0];
            SkillDatas = new SkillData[0];
            Items = new ItemPrototype[0];
            Formulas = new ItemFormula[0];
        }

        public TownData FindTown(string name)
        {
            return (from e in this.Towns where e.Name == name select e).Single();
        }
        public EntityData FindEntity(ENTITY name)
        {
            return (from e in this.Entitys where e.Name == name select e).Single();
        }

        public SkillData FindSkill(ACTOR_STATUS_TYPE name)
        {
            return (from e in this.SkillDatas where e.Id == name select e).Single();
        }

        public ItemPrototype FindItem(string name)
        {
            return (from e in this.Items where e.Name == name select e).Single();
        }

        public EntityGroupLayout FindEntityGroupLayout(string id)
        {
            return (from e in this.EntityGroupLayouts where e.Id == id select e).Single();
        }
    }
}