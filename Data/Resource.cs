using System;
using System.Linq;
namespace Regulus.Project.ItIsNotAGame1.Data
{
    public class Resource : Utility.Singleton<Resource>
    {
        public EntityData[] Entitys;
        public SkillData[] SkillDatas;

        public ItemPrototype[] Items;

        public ItemFormula[] Formulas;

        public EntityData FindEntity(ENTITY name)
        {
            return (from e in this.Entitys where e.Name == name select e).Single();
        }

        public SkillData FindSkill(ACTOR_STATUS_TYPE name)
        {
            return (from e in this.SkillDatas where e.Id == name select e).Single();
        }

        
    }
}