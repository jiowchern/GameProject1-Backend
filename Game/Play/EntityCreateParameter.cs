using Regulus.CustomType;
using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class EntityCreateParameter    
    {
        public ENTITY EntityType { get; set; }

        public Vector2 Position { get; set; }

        public float Direction { get; set; }
    }
}