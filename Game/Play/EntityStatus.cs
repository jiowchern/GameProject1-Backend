using System;

using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class EntityStatus 
    {
        private readonly Entity _Entity;

        

        public EntityStatus(Entity entity)
        {
            _Entity = entity;
        }

        public Vector2 GetVelocity(float delta_time)
        {
            return _ToVector(_Entity.Direction) * delta_time;
        }

        public void Move(float angle)
        {
            _Entity.SetMove(angle);            
        }

        private Vector2 _ToVector(float angle)
        {
            var radians = angle * 0.0174532924;
            return new Vector2((float)Math.Cos(radians), (float)-Math.Sin(radians));
        }
    }
}