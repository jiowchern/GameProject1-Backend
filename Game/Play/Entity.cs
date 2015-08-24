using System;

using Regulus.CustomType;
using Regulus.Extension;
using Regulus.Project.ItIsNotAGame1.Data;
namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Entity : IIndividual
    {

        
        private enum TRIGGER
        {
            MOVE
        };

        private readonly Flag<TRIGGER> _Trigger;        

        private readonly Polygon _Mesh;
        private readonly Guid _Id;
        
        public Entity(Polygon mesh , string name)
        {
            _Trigger = new Flag<TRIGGER>();
            _Id = Guid.NewGuid();
            
            _Name = name;
            _Mesh = mesh;            
        }

        System.Guid IVisible.Id
        {
            get { return _Id; }
        }

        private string _Name;
        string IVisible.Name {
            get { return _Name; }
        }

        private event Action<Vector2, Vector2> _MoveEvent;

        event Action<Vector2, Vector2> IVisible.MoveEvent
        {
            add { this._MoveEvent += value; }
            remove { this._MoveEvent -= value; }
        }

        Vector2 IVisible.Position { get { return _Mesh.Center; } }

        CustomType.Rect IIndividual.Bounds
        {
            get { return _Mesh.Points.ToRect(); }
        }

        Polygon IIndividual.Mesh {
            get { return _Mesh; }
        }

        private System.Action _BoundsEvent;

        private Vector2 _PreviousVelocity;

        public Guid Id { get { return _Id; } }

        public float Direction { get; private set; }

        event System.Action IIndividual.BoundsEvent
        {
            add { _BoundsEvent += value; }
            remove { _BoundsEvent -= value; }
        }

        public void UpdatePosition(Vector2 velocity)
        {
            _Mesh.Offset(velocity);
            if(_BoundsEvent != null)
                _BoundsEvent.Invoke();

            if (_Trigger[TRIGGER.MOVE])
            {
                if(_MoveEvent != null)
                    _MoveEvent.Invoke(_Mesh.Center, velocity);                
                _Trigger[TRIGGER.MOVE] = false;
            }
            
        }

        public void SetMove(float angle)
        {
            Direction = angle;
        }
    }
}