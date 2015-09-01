using System;

using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class EntityProvider 
    {
        public static Entity Create(GamePlayerRecord record)
        {
            var data = Singleton<Resource>.Instance.FindEntity(record.Entity);
            return new Entity(data.Mesh.Clone() , record);
        }
        
    }
}