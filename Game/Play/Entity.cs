using System;
using System.ComponentModel;

using Regulus.CustomType;
using Regulus.Extension;
using Regulus.Project.ItIsNotAGame1.Data;
namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Entity : IIndividual 
    {
        Rect _Bound;
        private readonly Polygon _Mesh;
        private readonly Guid _Id;

        private float _Speed;

        private float _View;

        private float _DetectionRange;

        public Inventory Bag { get; private set; }

        public Entity(Polygon mesh, GamePlayerRecord record)
        {
            this._Id = Guid.NewGuid();
            this._View = 30.0f;
            _DetectionRange = 1.0f;
            this._Record = record;

            this._Mesh = mesh;

            this._Bound = this._BuildBound(this._Mesh);

            Bag = new Inventory();
        }

        

        ENTITY IVisible.EntityType { get { return this._Record.Entity; } }

        Guid IVisible.Id
        {
            get { return this._Id; }
        }

        private GamePlayerRecord _Record;
        string IVisible.Name
        {
            get { return this._Record.Name; }
        }

        private event Action<Vector2, float, float , float> _MoveEvent;

        event Action<Vector2, float, float, float> IVisible.MoveEvent
        {
            add { this._MoveEvent += value; }
            remove { this._MoveEvent -= value; }
        }

        Vector2 IVisible.Position { get { return this._Mesh.Center; } }

        Rect IIndividual.Bounds
        {
            get
            {
                
                return this._Bound;
            }
        }

        private Rect _BuildBound(Polygon mesh)
        {
            var center = mesh.Center;
            var rect = new Rect(center.X, center.Y, this._View, this._View);
            return rect.LeftToCenter();
        }

        Polygon IIndividual.Mesh
        {
            get { return this._Mesh; }
        }

        private Action _BoundsEvent;
        private float _Trun;

        

        public Guid Id { get { return this._Id; } }

        public float Direction { get; private set; }

        event Action IIndividual.BoundsEvent
        {
            add { this._BoundsEvent += value; }
            remove { this._BoundsEvent -= value; }
        }

        void IIndividual.SetPosition(float x, float y)
        {
            if (x == 0 && y == 0)
                return;

            var offset = this._Mesh.Center + new Vector2(x, y);
            this._Mesh.Offset(offset);
            this._Bound = this._BuildBound(this._Mesh);
            if (this._BoundsEvent != null)
                this._BoundsEvent.Invoke();
        }

        Item[] IIndividual.Stolen()
        {
            return new [] { new Item() { Id = Guid.NewGuid() , Weight = 5 , Name = "探索到的物品"} } ;
        }

        public void UpdatePosition(Vector2 velocity)
        {
            if (velocity.X == 0 && velocity.Y == 0)
                return;

            this._Mesh.Offset(velocity);
            this._Bound = this._BuildBound(this._Mesh);
            if (this._BoundsEvent != null)
                this._BoundsEvent.Invoke();
            

        }

        public void Stop()
        {
            this._Speed = 0.0f ;
            this._SetMove(0);
        }

        public void Move(float angle , bool run)
        {
            this._Speed = 1.0f + (run? 3.0f : 0.0f);
            this._SetMove(angle);
        }

        private void _SetMove(float angle)
        {
            this.Direction = (this.Direction + angle) % 360;
            if (this._MoveEvent != null)
            {
                this._MoveEvent.Invoke(this._Mesh.Center, this._Speed, this.Direction, this._Trun);
            }            
        }

        public Vector2 GetVelocity(float delta_time)
        {
            this.Direction = (this.Direction + (this._Trun*delta_time)) % 360;
            return this._ToVector(this.Direction) * delta_time * this._Speed;
        }

        private Vector2 _ToVector(float angle)
        {
            var radians = angle * 0.0174532924;
            return new Vector2((float)Math.Cos(radians), (float)-Math.Sin(radians));
        }

        

        private Rect _BuildVidw()
        {
            var center = this._Mesh.Center;
            var hw = this._View / 2;
            var hh = this._View / 2;
            var rect = new Rect(center.X - hw, center.Y - hh, this._View , this._View);
            return rect;
        }

        internal void Trun(int trun)
        {
            this._Trun = trun;

            if (this._MoveEvent != null)
            {
                this._MoveEvent.Invoke(this._Mesh.Center,  this._Speed, this.Direction , this._Trun ) ;
            } 
        }

        public Polygon GetExploreBound()
        {
            var ext = new ExtendPolygon(_Mesh , _DetectionRange );
            return ext.Result;
        }

        public Rect GetView()
        {
            return _BuildVidw();
        }

        
    }
}