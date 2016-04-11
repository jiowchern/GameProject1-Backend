using System;

using Regulus.CustomType;
using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public interface IMapGate
    {
        void Left(Entity player);

        void Join(Entity player);

        void Pass(Vector2 position, ENTITY[] entitys);

        void Pass(Vector2 position, Guid id);

        Guid Spawn(ENTITY type);
        Guid[] SpawnField(ENTITY[] types);
    }
}