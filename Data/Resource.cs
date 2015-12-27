using System;
using System.Linq;
namespace Regulus.Project.ItIsNotAGame1.Data
{
    public class Resource : Utility.Singleton<Resource>
    {
        public EntityData[] Entitys;

        public EntityData FindEntity(ENTITY name)
        {
            return (from e in this.Entitys where e.Name == name select e).Single();
        }        
    }
}