using System;

using Regulus.CustomType;
using Regulus.Extension;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Entity : IVisible
    {
        private readonly Polygon _Mesh;
        private readonly Guid _Id;
        
        public Entity(Polygon mesh)
        {
            _Mesh = mesh;
            _Id = Guid.NewGuid();
        }

        System.Guid IVisible.Id
        {
            get { return _Id; }
        }

        CustomType.Rect IVisible.Bounds
        {
            get { return _Mesh.Points.ToRect(); }
        }

        Polygon IVisible.Mesh
        {
            get { return _Mesh; }
        }

        private System.Action _BoundsEvent;
        event System.Action IVisible.BoundsEvent
        {
            add { _BoundsEvent += value; }
            remove { _BoundsEvent -= value; }
        }
    }
}