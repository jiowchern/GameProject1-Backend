﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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

        private SkillData[] _Datas  ;
/*        {
            new SkillData() { Id = ACTOR_STATUS_TYPE.BATTLE_AXE_BLOCK , Total =1.833f },
            new SkillData() { Id = ACTOR_STATUS_TYPE.BATTLE_AXE_ATTACK1 , Total =2.4f },
            new SkillData() { Id = ACTOR_STATUS_TYPE.BATTLE_AXE_ATTACK2 , Total =3.167f }
        };*/

        public Inventory Bag { get; private set; }

        public Entity(Polygon mesh, GamePlayerRecord record )
        {
            _SkillCasters = new Dictionary<Guid, SkillCaster>();
            //_Datas = Resource.Instance.SkillDatas;
            _Datas = new []
            {
                new SkillData() { Id = ACTOR_STATUS_TYPE.BATTLE_AXE_BLOCK , Total =1.833f },
                new SkillData() { Id = ACTOR_STATUS_TYPE.BATTLE_AXE_ATTACK1 , Total =2.4f },
                new SkillData() { Id = ACTOR_STATUS_TYPE.BATTLE_AXE_ATTACK2 , Total =3.167f }
            };
            this._Id = Guid.NewGuid();
            this._View = 30.0f;
            _DetectionRange = 1.0f;
            this._Record = record;

            this._Mesh = mesh;

            this._Bound = this._BuildBound(this._Mesh);

            Bag = new Inventory();

            _IdleStatus = ACTOR_STATUS_TYPE.NORMAL_IDLE;
            
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

        private event Action<VisibleStatus> _StatusEvent;

        event Action<VisibleStatus> IVisible.StatusEvent
        {
            add { this._StatusEvent += value; }
            remove { this._StatusEvent -= value; }
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

        private ACTOR_STATUS_TYPE _IdleStatus;

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

        private Dictionary<Guid, SkillCaster> _SkillCasters;
        void IIndividual.AttachDamage(Guid id, SkillCaster caster)
        {
            if(_SkillCasters.ContainsKey(id) == false)
                _SkillCasters.Add(id , caster);
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
        internal void Trun(int trun)
        {
            this._Trun = trun;
            _InvokeStatusEvent(_IdleStatus);
        }
        public void Move(float angle , bool run)
        {
            this._Speed = 1.0f + (run? 3.0f : 0.0f);
            this._SetMove(angle );
        }

        private void _SetMove(float angle)
        {
            this.Direction = (this.Direction + angle) % 360;
            _InvokeStatusEvent(_IdleStatus);
        }

        private void _InvokeStatusEvent(ACTOR_STATUS_TYPE type)
        {
            if (this._StatusEvent != null)
            {
                var status = new VisibleStatus()
                {
                    Status = type,
                    StartPosition = _Mesh.Center,
                    Speed = _Speed,
                    Direction = this.Direction,
                    Trun = _Trun
                };
                this._StatusEvent.Invoke(status);
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

        

        public Polygon GetExploreBound()
        {
            var ext = new ExtendPolygon(_Mesh , _DetectionRange );
            return ext.Result;
        }

        public Rect GetView()
        {
            return _BuildVidw();
        }

        public void Normal()
        {
            _IdleStatus = ACTOR_STATUS_TYPE.NORMAL_IDLE;
            Stop();            
        }

        public void Explore()
        {
            _InvokeStatusEvent(ACTOR_STATUS_TYPE.NORMAL_EXPLORE);            
        }

        public void Battle()
        {
            _IdleStatus = ACTOR_STATUS_TYPE.BATTLE_AXE_IDLE;
            Stop();
        }

        public ActorSkill FindSkill()
        {            
            return new ActorSkill(_Datas);
        }

        public void CastBegin(ACTOR_STATUS_TYPE id)
        {
            _Cast(id);
            
        }

        private void _Cast(ACTOR_STATUS_TYPE id)
        {
            _Speed = 0.0f;
            _Trun = 0.0f;            
            _InvokeStatusEvent(id);
        }

        public void CastEnd(ACTOR_STATUS_TYPE id)
        {            
        }

        public IEnumerable<SkillCaster> DequeueCaster()
        {
            var values = _SkillCasters.Values;
            _SkillCasters.Clear();
            return values;
        }

        public void Damage()
        {
            _InvokeStatusEvent(ACTOR_STATUS_TYPE.DAMAGE1);
        }

        public SkillCaster GetDamagrCaster()
        {
            var data = _Datas.First((s) => s.Id == ACTOR_STATUS_TYPE.DAMAGE1);
            return new SkillCaster(data , new Determination(data.Lefts , data.Rights , data.Total , data.Begin , data.End));
        }
    }
}