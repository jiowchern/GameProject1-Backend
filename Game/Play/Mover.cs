using System;
using System.Collections.Generic;
using System.Linq;
using Regulus.Extension;
using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Mover
    {

        private Entity _Entity;

        private IIndividual _Individual { get { return _Entity; } }
        private Guid _Id;

        public Mover(Entity entity)
        {
            _Entity = entity;
        }
        public IObservable GetOrbit(Vector2 velocity)
        {                            
            return new Orbit(_Individual.Mesh, velocity);
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
        
        public void Set(Vector2 velocity)
        {
            _Entity.UpdatePosition(velocity);
        }

        public bool Move(Vector2 velocity, IEnumerable<IIndividual> entitys )
        {
            var polygon = _GetThroughRange(velocity);

            if (entitys.Any(x => _Collide(x, polygon )))
            {
                return false;
            }
            Set(velocity);
            return true;
        }        

        private Polygon _GetThroughRange(Vector2 velocity)
        {
            var after = _Individual.Mesh.Clone();
            after.Offset(velocity);

            List<Vector2> points = new List<Vector2>();
            points.AddRange(_Individual.Mesh.Points);
            points.AddRange(after.Points);
            var polygon = new Polygon(points.FindHull().ToArray());            
            return polygon;
        }

        private bool _Collide(IIndividual individual, Polygon polygon)
        {
            var result = Polygon.Collision(polygon, individual.Mesh, new Vector2());
            return result.Intersect;
        }
    }
}