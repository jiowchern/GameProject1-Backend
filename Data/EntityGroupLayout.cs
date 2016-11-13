using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    [Serializable]
    public class EntityGroupLayout
    {
        public string Id;
        public EntityLayout[] Entitys;
        public ChestLayout[] Chests;
        public StaticLayout[] Statics;
        public WallLayout[] Walls;
        public ResourceLayout[] Resources;

        public EnteranceLayout[] Enterances;
        

        public StrongholdLayout[] Strongholds;

        public FieldLayout[] Fields { get; set; }
    }

    public struct WallLayout 
    {
        public Guid Owner;
    }

    [Serializable]
    public struct FieldLayout
    {
        public Guid Owner;

        public ENTITY[] Kinds;
    }

    [Serializable]
    public struct StrongholdLayout
    {
        public Guid Owner;

        public ENTITY[] Kinds;
    }
    [Serializable]
    public struct EnteranceLayout
    {
        public Guid Owner;

        public ENTITY[] Kinds;

    }

    [Serializable]
    public struct ResourceItem
    {
        public string Name;        
    }
    [Serializable]
    public struct ResourceLayout
    {
        public Guid Owner;
        public ResourceItem[] Items;
    }

    [Serializable]
    public struct StaticLayout
    {
        public Guid Owner;

        public Polygon Body;
    }

    [Serializable]
    public struct ChestLayout
    {
        public Guid Owner;
        public Guid Exit;
        public Guid Debirs;
        public Guid Gate;
    }
}
