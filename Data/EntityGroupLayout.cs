using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    [Serializable]
    public class EntityGroupLayout
    {        
        public EntityLayout[] Entitys;
        public ChestLayout[] Chests;
        public StaticLayout[] Statics;
        public string Id;
    }

    [Serializable]
    public class StaticLayout
    {
        public Guid Owner;
    }

    [Serializable]
    public class ChestLayout
    {
        public Guid Owner;
        public Guid Exit;
        public Guid Debirs;
        public Guid Gate;
    }
}
