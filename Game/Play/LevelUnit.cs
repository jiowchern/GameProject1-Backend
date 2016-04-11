using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class LevelUnit
    {
        public readonly LEVEL_UNIT Type;

        public readonly Vector2 Center;

        public readonly float Direction;

        public LevelUnit(LEVEL_UNIT type, Vector2 center, float direction)
        {
            Type = type;
            Center = center;
            Direction = direction;
        }
    }
}