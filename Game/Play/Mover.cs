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

        private IIndividual _Individual { get { return this._Entity; } }        

        public Mover(Entity entity)
        {
            this._Entity = entity;
        }
        public Rect GetOrbit(Vector2 velocity)
        {                            
            return new Orbit(this._Individual.Mesh, velocity).Vision;
        }

        public class Orbit 
        {
            private readonly Rect _Rect;

            public Orbit(Polygon body, Vector2 velocity)
            {
                List<Vector2> points = new List<Vector2>();
                points.AddRange(body.Points);

                var polygon = body.Clone();
                polygon.Offset(velocity);

                points.AddRange(polygon.Points);
                this._Rect = points.ToRect();
            }

            public Rect Vision { get { return this._Rect; } }
        }
        
        public void Set(Vector2 velocity)
        {
            this._Entity.UpdatePosition(velocity);
        }

        public IEnumerable<IIndividual> Move(Vector2 velocity, IEnumerable<IIndividual> entitys )
        {
            var polygon = this._GetThroughRange(velocity);

            var targets = from x in entitys
                          where
                              x.EntityType != Data.ENTITY.ACTOR1 

                              && x.EntityType != Data.ENTITY.ENTRANCE
                              && x.EntityType != Data.ENTITY.ACTOR2
                              && this._Collide(x, polygon)
                          select x;
            if (targets.Any() == false)
            {
                this.Set(velocity);
            }            
            return targets;
        }        

        private Polygon _GetThroughRange(Vector2 velocity)
        {
            var after = this._Individual.Mesh.Clone();
            after.Offset(velocity);

            List<Vector2> points = new List<Vector2>();
            points.AddRange(this._Individual.Mesh.Points);
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