using System;
using System.Collections.Generic;
using System.Linq;
using Regulus.Extension;
using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Mover
    {
        private Regulus.CustomType.Polygon _Body;

        private Guid _Id;

        public Mover(Guid id , Regulus.CustomType.Polygon body)
        {
            _Id = id;
            _Body =  body;
        }
        public IObservable GetOrbit(Vector2 velocity)
        {                            
            return new Orbit(_Body, velocity);
        }

        public class Orbit : IObservable
        {
            private readonly Rect _Rect;

            public Orbit(Polygon body, Vector2 velocity)
            {
                List<Vector2> points = new List<Vector2>();
                points.AddRange(body.Points);

                var polygon = body.Clone();
                polygon.Offset(velocity);

                points.AddRange(polygon.Points);
                _Rect = points.ToRect();
            }

            Rect IObservable.Vision { get { return _Rect; } }
        }

        private Vector2 _GetVector(float angle)
        {            
            var radians =angle * 0.0174532924;
            return new Vector2((float)Math.Cos(radians) , (float)- Math.Sin(radians));
        }

        public bool Collide(IVisible entity)
        {
            var result = Polygon.Collision(_Body, entity.Mesh , new Vector2());
            return result.Intersect;
        }

        public void Set(Vector2 velocity)
        {
            _Body.Offset(velocity);
        }

        public bool Move(Vector2 velocity, IEnumerable<IVisible> entitys )
        {
            var polygon = _GetThroughRange(velocity);

            if (entitys.Any(x => _Collide(x, polygon)))
            {
                return false;
            }
            this.Set(velocity);
            return true;
        }        

        private Polygon _GetThroughRange(Vector2 velocity)
        {
            var after = _Body.Clone();
            after.Offset(velocity);

            List<Vector2> points = new List<Vector2>();
            points.AddRange(_Body.Points);
            points.AddRange(after.Points);
            var polygon = new Polygon(points.ToArray());
            polygon.Convex();
            return polygon;
        }

        private bool _Collide(IVisible visible, Polygon polygon)
        {
            var result = Polygon.Collision(polygon, visible.Mesh, new Vector2());
            return result.Intersect;
        }
    }
}