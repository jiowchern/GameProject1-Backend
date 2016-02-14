using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Regulus.CustomType;
using Regulus.Extension;
using Regulus.Project.ItIsNotAGame1.Data;
namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Entity : IIndividual, IDevelopActor
        , IPlayerProperys
    {
        private ENTITY _EntityType;
        private string _Name;
        Rect _Bound;
        private readonly Polygon _Mesh;
        private readonly Guid _Id;

        private float _Speed;

        private IEnumerable<IIndividual> _CollisionTargets;

        private float _BaseView;

        private float _IlluminateView;

        private float _View
        {
            get
            {
                return _BaseView + _IlluminateView;
            }
        }

        public float ScanLength { get { return _View / 2; } }

        private float _DetectionRange;

        private SkillData[] _Datas;


        public Inventory Bag { get; private set; }

        private Concierge _Concierge;

        

        public Entity(ENTITY type, string name, Polygon mesh, Concierge concierge)
            : this(type, name, mesh)
        {
            _Concierge = concierge;


        }

        public Entity(ENTITY type, Polygon mesh) : this(mesh)
        {
            _Name = "";
            _EntityType = type;
            _Datas = Resource.Instance.SkillDatas;

            this._Id = Guid.NewGuid();
            this._BaseView = 10.0f;
            _DetectionRange = 1.0f;

            Bag = new Inventory();
            Equipment = new Equipment(this);
            Equipment.AddEvent += _BroadcastEquipEvent;
            Equipment.RemoveEvent += _BroadcastEquipEvent;

            _Status = ACTOR_STATUS_TYPE.NORMAL_IDLE;
        }
        public Entity(ENTITY type, string name, Polygon mesh) : this(type, mesh)
        {
            _Name = name;
        }

        public Entity(Polygon mesh)
        {
            this._Mesh = mesh;
            this._Bound = this._BuildBound(this._Mesh);
            _CollisionTargets = new IIndividual[0];


        
            _SkillOffsetVector = new Vector2();

            _BaseSpeed = 1.0f;
        }

        private void _BroadcastEquipEvent(Guid obj)
        {
            this._BroadcastEquipEvent();
        }

        private void _BroadcastEquipEvent()
        {
            var equipStatus = from item in this.Equipment.GetAll()
                              select new EquipStatus()
                              {
                                  Item = item.Name,
                                  Part = item.GetEquipPart()
                              };

            this._EquipEvent(equipStatus.ToArray());
        }

        private void _BroadcastEquipEvent(Item obj)
        {

            _BroadcastEquipEvent();
        }


        ENTITY IVisible.EntityType { get { return _EntityType; } }

        Guid IVisible.Id
        {
            get { return this._Id; }
        }



        public float Strength(float val)
        {
            
            _Strength += val;
            if (_Strength > 3.0f)
                _Strength = 3.0f;

            return _Strength;
        }

        float IPlayerProperys.Strength {
            get { return _Strength; }
        }

        string IVisible.Name
        {
            get { return _Name; }
        }

        float IVisible.View { get { return _View; } }

        private event Action<EquipStatus[]> _EquipEvent;

        event Action<EquipStatus[]> IVisible.EquipEvent
        {
            add { this._EquipEvent += value; }
            remove { this._EquipEvent -= value; }
        }

        private event Action<VisibleStatus> _StatusEvent;

        event Action<VisibleStatus> IVisible.StatusEvent
        {
            add { this._StatusEvent += value; }
            remove { this._StatusEvent -= value; }
        }

        Vector2 IVisible.Position { get { return this._Mesh.Center; } }

        void IVisible.QueryStatus()
        {
            var status = new VisibleStatus()
            {
                Status = _Status,
                StartPosition = _Mesh.Center,
                Speed = _Speed,
                Direction = Direction,
                Trun = _Trun,
                SkillOffect = _SkillOffsetVector
            };

            this._StatusEvent.Invoke(status);        

            _BroadcastEquipEvent();
        }

        private event Action<string> _OnTalkMessageEvent;

        event Action<string> IVisible.TalkMessageEvent
        {
            add { this._OnTalkMessageEvent += value; }
            remove { this._OnTalkMessageEvent -= value; }
        }

        Rect IIndividual.Bounds
        {
            get
            {

                return this._Bound;
            }
        }

        private Rect _BuildBound(Polygon mesh)
        {

            return mesh.Points.ToRect();
        }

        Polygon IIndividual.Mesh
        {
            get { return this._Mesh; }
        }

        private Action _BoundsEvent;
        private float _Trun;

        private ACTOR_STATUS_TYPE _Status;

        public Guid Id { get { return this._Id; } }

        public float Direction { get; private set; }

        public Equipment Equipment { get; set; }

        public float Speed { get { return _Speed; } }

        event Action IIndividual.BoundsEvent
        {
            add { this._BoundsEvent += value; }
            remove { this._BoundsEvent -= value; }
        }

        void IIndividual.SetPosition(float x, float y)
        {

            var offset = new Vector2(x, y) - this._Mesh.Center;
            this._Mesh.Offset(offset);
            this._Bound = this._BuildBound(this._Mesh);
            if (this._BoundsEvent != null)
                this._BoundsEvent.Invoke();
        }

        Item[] IIndividual.Stolen()
        {
            _Status = ACTOR_STATUS_TYPE.CHEST_OPEN;
            _InvokeStatusEvent();
            var itemProivder = new ItemProvider();
            return itemProivder.FromStolen();
        }



        bool IIndividual.IsBlock()
        {
            return _Block;
        }

        Concierge IIndividual.GetConcierge()
        {
            return _Concierge;
        }

        private bool _Block;

        private int _DamageCount;

        private Vector2 _SkillOffsetVector;

        private float _BaseSpeed;

        private float _Strength;

        void IIndividual.AttachDamage(bool smash)
        {
            if (smash)
                _DamageCount += 3;
            else
                _DamageCount++;
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
            this._Speed = 0.0f;
            _InvokeStatusEvent();
        }
        internal void Trun(float trun)
        {
            if (trun != _Trun)
            {
                this._Trun = trun;
                _InvokeStatusEvent();
            }

        }

        public void Back(float speed)
        {            
            if (speed != _Speed)
            {
                this._Speed = speed * _BaseSpeed;
                _InvokeStatusEvent();
            }
        }

        public void Move(float angle, float speed)
        {            
            if (speed != _Speed)
            {
                this._Speed = speed * _BaseSpeed;
                _AddDirection(angle);
                _InvokeStatusEvent();
            }
        }

        private void _AddDirection(float angle)
        {
            Direction = (this.Direction + angle) % 360;
            Direction += 360; 
            Direction %= 360; 
        }

        private void _InvokeStatusEvent()
        {

            if (_StatusEvent != null)
            {
                var status = new VisibleStatus()
                {
                    Status = _Status,
                    StartPosition = _Mesh.Center,
                    Speed = _Speed,
                    Direction = Direction,
                    Trun = _Trun,
                    SkillOffect = _SkillOffsetVector
                };
                this._StatusEvent.Invoke(status);                

            }
        }
        public Vector2 GetDirectionVector()
        {
            return this._ToVector(this.Direction);
        }

        public Vector2 GetVelocity(float delta_time)
        {
            var skill = _SkillOffsetVector * delta_time;
            return this._ToVector(this.Direction) * delta_time * this._Speed + skill;
        }

        public void TrunDirection(float delta_time)
        {
            _AddDirection(_Trun * delta_time);
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
            var rect = new Rect(center.X - hw, center.Y - hh, this._View, this._View);
            return rect;
        }



        public Polygon GetExploreBound()
        {
            var ext = new ExtendPolygon(_Mesh, _DetectionRange);
            return ext.Result;
        }

        public Rect GetView()
        {
            return _BuildVidw();
        }

        public void Normal()
        {
            _Status = ACTOR_STATUS_TYPE.NORMAL_IDLE;
            Stop();
        }

        public void Explore()
        {
            _Status = ACTOR_STATUS_TYPE.NORMAL_EXPLORE;
            _InvokeStatusEvent();
        }
        private ACTOR_STATUS_TYPE _GetWeaponIdle(ITEM_FEATURES features)
        {
            switch (features)
            {
                case ITEM_FEATURES.WEAPON_AXE:
                    return ACTOR_STATUS_TYPE.BATTLE_AXE_IDLE;
                case ITEM_FEATURES.WEAPON_TWOHANDSWORD:
                    return ACTOR_STATUS_TYPE.TWO_HAND_SWORD_IDLE;
                default:
                    return ACTOR_STATUS_TYPE.MELEE_IDLE;
            }
        }




        public void CastBegin(ACTOR_STATUS_TYPE id)
        {
            _Cast(id);

        }

        private void _Cast(ACTOR_STATUS_TYPE id)
        {
            _Speed = 0.0f;
            _Trun = 0.0f;
            _Status = id;
            _InvokeStatusEvent();
        }

        public void CastEnd(ACTOR_STATUS_TYPE id)
        {

        }

        public int HaveDamage()
        {
            var values = _DamageCount;
            _DamageCount = 0;
            return values;
        }

        public void Damage()
        {
            _Speed = 0.0f;
            _Trun = 0.0f;
            _Status = ACTOR_STATUS_TYPE.DAMAGE1;
            _InvokeStatusEvent();

        }

        public SkillCaster GetDamagrCaster()
        {
            var data = _Datas.First((s) => s.Id == ACTOR_STATUS_TYPE.DAMAGE1);
            return new SkillCaster(data, new Determination(data.Lefts, data.Rights, data.Total, data.Begin, data.End));
        }

        public void SetBlock(bool set)
        {
            _Block = set;
        }

        public Vector2 GetPosition()
        {
            return _Mesh.Center;
        }

        public void SetRotation(float next_float)
        {
            _AddDirection(next_float);
            _Mesh.RotationByDegree(next_float);
        }

        public void Talk(string message)
        {
            if (_OnTalkMessageEvent != null)
                _OnTalkMessageEvent(message);
        }

        public void Make()
        {
            _Speed = 0.0f;
            _Trun = 0.0f;
            _InvokeStatusEvent();
        }

        public ItemFormula[] GetFormulas()
        {
            return Resource.Instance.Formulas;
        }


        public void SetCollisionTargets(IEnumerable<IIndividual> hitthetargets)
        {
            _CollisionTargets = hitthetargets;
        }

        public void ClearCollisionTargets()
        {
            _CollisionTargets = new IIndividual[0];
        }

        public IEnumerable<IIndividual> GetCollisionTargets()
        {
            return _CollisionTargets;
        }

        public void SetDirection(float dir)
        {
            _AddDirection(dir);
            _InvokeStatusEvent();
        }

        public ACTOR_STATUS_TYPE GetIdle()
        {
            return _Status;
        }

        public void SetEquipView(float view)
        {
            _IlluminateView = view;
        }

        void IDevelopActor.SetBaseView(float range)
        {
            _BaseView = range;
        }

        void IDevelopActor.SetSpeed(float speed)
        {
            _BaseSpeed = speed;
        }

        public SkillCaster GetBattleCaster()
        {

            ACTOR_STATUS_TYPE status = ACTOR_STATUS_TYPE.MELEE_IDLE;
            var item = this.Equipment.Find(EQUIP_PART.RIGHT_HAND);
            if (item.HasValue)
            {
                status = _GetWeaponIdle(item.Value.GetPrototype().Features);
            }

            var data = _Datas.First((s) => s.Id == status);
            return new SkillCaster(data, new Determination(data.Lefts, data.Rights, data.Total, data.Begin, data.End));
        }

        public void SetSkillVelocity(float direction, float speed)
        {
            var dir = ((Direction + direction) + 360.0f) % 360.0f;
            _SkillOffsetVector = _ToVector(dir) * speed;
        }

        public void ClearOffset()
        {
            this._Speed = 0.0f;
            _SkillOffsetVector = new Vector2();
            _InvokeStatusEvent();
        }

        
    }
}