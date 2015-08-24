using System;
using System.Linq;
namespace Regulus.Project.ItIsNotAGame1.Data
{
    public class Resource : Regulus.Utility.Singleton<Resource>
    {
        public EntityData[] Entitys;

        public EntityData FindEntity(string name)
        {
            return (from e in Entitys where e.Name == name select e).SingleOrDefault();
        }        
    }
}