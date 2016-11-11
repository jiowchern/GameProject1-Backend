using System;

using Regulus.CustomType;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class EntityProvider 
    {
        public static Entity Create(GamePlayerRecord record)
        {
            var data = Singleton<Resource>.Instance.FindEntity(record.Entity);
            return new Entity(data, record.Name);
        }

        public static Entity Create(ENTITY type)
        {
            var data = Singleton<Resource>.Instance.FindEntity(type);
            return new Entity(data);
        }

        public static Entity CreateStatic(Polygon mesh)
        {
            return new Entity(new EntityData() { CollisionRotation = false , Name = ENTITY.STATIC , Mesh = mesh});
        }
    }
}