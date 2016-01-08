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
            return new Entity(record.Entity , record.Name, data.Mesh.Clone()  );
        }

        public static Entity Create(ENTITY type)
        {
            var data = Singleton<Resource>.Instance.FindEntity(type);
            return new Entity(type,  data.Mesh.Clone());
        }

        public static Entity CreateEnterance(ENTITY[] types)
        {
            var data = Singleton<Resource>.Instance.FindEntity(ENTITY.ENTRANCE);
            var mesh = data.Mesh.Clone();
            return new Entity(ENTITY.ENTRANCE, "" , mesh , new Concierge(mesh , types));
        }
    }
}