using System;
using System.Collections.Generic;

using Regulus.Extension;
using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Mover
    {
        private Regulus.CustomType.Polygon _Body;
        private float _Speed;

        public Mover(Regulus.CustomType.Polygon body, float speed)
        {                        
            _Body =  body;
            _Speed = speed;
        }
        public IObservable GetOrbit(float angle , float second )
        {
            var vector = _GetVector(angle);
            return null;


        }

        private Vector2 _GetVector(float angle)
        {            
            var radians =angle * 0.0174532924;
            return new Vector2((float)Math.Cos(radians) , (float)- Math.Sin(radians));
        }

    }
}