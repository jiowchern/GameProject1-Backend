using System;

using Regulus.CustomType;
using Regulus.Extension;
using Regulus.Project.ItIsNotAGame1.Data;
namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Entity : IIndividual , IObservable
    {


        private enum TRIGGER
        {
            MOVE
        };

        private readonly Flag<TRIGGER> _Trigger;

        private readonly Polygon _Mesh;
        private readonly Guid _Id;

        private float _Speed;

        private float _View;

        public Entity(Polygon mesh, GamePlayerRecord record)
        {
            _Trigger = new Flag<TRIGGER>();
            _Id = Guid.NewGuid();
            _View = 30.0f;
            this._Record = record;
            
            _Mesh = mesh;
        }

        ENTITY IVisible.EntityType { get { return _Record.Entity; } }

        System.Guid IVisible.Id
        {
            get { return _Id; }
        }

        private GamePlayerRecord _Record;
        string IVisible.Name
        {
            get { return _Record.Name; }
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
            get
            {
                var center = _Mesh.Center;
                var rect = new Rect(center.X, center.Y, _View, _View);
                return rect.LeftToCenter();
            }
        }

        Polygon IIndividual.Mesh
        {
            get { return _Mesh; }
        }

        private System.Action _BoundsEvent;


        public Guid Id { get { return _Id; } }

        public float Direction { get; private set; }

        event System.Action IIndividual.BoundsEvent
        {
            add { _BoundsEvent += value; }
            remove { _BoundsEvent -= value; }
        }

        void IIndividual.SetPosition(float x, float y)
        {
            var offset = _Mesh.Center + new Vector2(x, y);
            _Mesh.Offset(offset);
            if (_BoundsEvent != null)
                _BoundsEvent.Invoke();
        }

        public void UpdatePosition(Vector2 velocity)
        {
            _Mesh.Offset(velocity);
            if (_BoundsEvent != null)
                _BoundsEvent.Invoke();
            

        }

        public void Stop()
        {
            _Speed = 0.0f;
            _SetMove(Direction);
        }

        public void Move(float angle)
        {
            _Speed = 1.0f;
            _SetMove(angle);
        }

        private void _SetMove(float angle)
        {
            this.Direction = angle;
            this._Trigger[TRIGGER.MOVE] = true;

            if (this._Trigger[TRIGGER.MOVE])
            {
                if (this._MoveEvent != null)
                {
                    this._MoveEvent.Invoke(this._Mesh.Center, this._ToVector(this.Direction) * this._Speed);
                }
                this._Trigger[TRIGGER.MOVE] = false;
            }
        }

        public Vector2 GetVelocity(float delta_time)
        {
            return _ToVector(Direction) * delta_time * _Speed;
        }

        private Vector2 _ToVector(float angle)
        {
            var radians = angle * 0.0174532924;
            return new Vector2((float)Math.Cos(radians), (float)-Math.Sin(radians));
        }

        Rect IObservable.Vision { get { return _BuildVidw(); } }

        private Rect _BuildVidw()
        {
            var center = _Mesh.Center;
            var hw = _View;
            var hh = _View;
            var rect = new Rect(center.X - hw, center.Y - hh, _View , _View);
            return rect;
        }
    }
}