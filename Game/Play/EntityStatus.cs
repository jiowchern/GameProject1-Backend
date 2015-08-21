using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class EntityStatus 
    {
        public Vector2 GetSpeed(float delta_time)
        {
            return new Vector2(1,1) * delta_time;
        }
    }
}