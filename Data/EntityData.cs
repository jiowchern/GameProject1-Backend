using System;

using ProtoBuf;

using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    [ProtoContract]
    [Serializable]
    public struct EntityData
    {
        [ProtoMember(1)]
        public ENTITY Name;
        [ProtoMember(2)]
        public Polygon Mesh;


        public static bool IsActor(ENTITY e)
        {
            return !IsWall(e);
        }

        public static bool IsWall(ENTITY e)
        {
            return e != Data.ENTITY.ACTOR1
                   && e != Data.ENTITY.ENTRANCE
                   && e != Data.ENTITY.ACTOR2
                   && e != Data.ENTITY.ACTOR3
                   && e != Data.ENTITY.ACTOR4
                   && e != Data.ENTITY.ACTOR5
                   && e != Data.ENTITY.DEBIRS;
        }

    }
}