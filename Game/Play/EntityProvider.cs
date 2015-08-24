using System;

using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class EntityProvider 
    {
        public static Entity Create(string entity_name ,string resource_name)
        {
            var data = Singleton<Resource>.Instance.FindEntity(resource_name);
            return new Entity(data.Mesh , entity_name);
        }
    }
}