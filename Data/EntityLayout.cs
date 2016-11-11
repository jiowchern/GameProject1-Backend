using System;

using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    [Serializable]
    public abstract class EntityLayout
    {
        public ENTITY Type;
        public Vector2 Position;

        public float Direction;


        public abstract EntityData Build();
    }
}