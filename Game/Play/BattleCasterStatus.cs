using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

using Regulus.CustomType;
using Regulus.Extension;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;



using Vector2 = Regulus.CustomType.Vector2;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class BattleCasterStatus : IStage, ICastSkill, IBattleSkill
    {
        private readonly ISoulBinder _Binder;

        private readonly Entity _Player;

        private readonly Map _Map;

        private readonly SkillCaster _Caster;

        public event Action<SkillCaster> NextEvent;
        public event Action DisarmEvent;
        public event Action BattleIdleEvent;

        private readonly HashSet<Guid> _Attacked;

        private readonly Regulus.Utility.TimeCounter _CastTimer;

        private float _CurrentCastTime;

        private readonly MoveController _MoveController;

        private Vector2 _DatumPosition;

        private bool _Overdraft;

        public BattleCasterStatus(ISoulBinder binder, Entity player, Map map, SkillCaster caster)
        {
            _CastTimer = new TimeCounter();
            _Attacked = new HashSet<Guid>();
            _Binder = binder;
            _Player = player;
            _Map = map;
            _Caster = caster;
            _MoveController = new MoveController(player);
        }

        void IStage.Enter()
        {
            _Binder.Bind<ICastSkill>(this);
            
            _Player.SetSkillVelocity(_Caster.GetShiftDirection()  , _Caster.GetShiftSpeed() );
            _Player.CastBegin(_Caster.Data.Id);

            _MoveController.Backward = _Caster.GetBackward();
            _MoveController.Forward = _Caster.GetForward();
            _MoveController.RunForward = _Caster.GetRunForward();
            _MoveController.TurnLeft = _Caster.GetTurnLeft();
            _MoveController.TurnRight = _Caster.GetTurnRight();

            _Binder.Bind<IMoveController>(_MoveController);            

            if (_Caster.CanDisarm())
            {
                _Binder.Bind<IBattleSkill>(this);
            }

            _CastTimer.Reset();

            _DatumPosition = _Player.GetPosition();
            var strength = _Player.Strength(-_Caster.Data.StrengthCost);
            _Overdraft = strength < 0.0f;
        }

        void IStage.Leave()
        {
            _Player.SetSkillVelocity(0,0);
            if (_Caster.CanDisarm())
            {
                _Binder.Unbind<IBattleSkill>(this);
            }
            _Binder.Unbind<IMoveController>(_MoveController);

            _Binder.Unbind<ICastSkill>(this);
            _Player.CastEnd(_Caster.Data.Id);

        }
        #region UnityDebugCode
        //Unity Debug Code
#if UNITY_EDITOR
        static public void _Draw(Polygon result,  float y, UnityEngine.Color color)
        {
            var points = (from p in result.Points select new UnityEngine.Vector3(p.X, y, p.Y)).ToArray();
            var len = points.Length;
            if (len < 2)
            {
                return;
            }
            for (int i = 0; i < len - 1; i++)
            {
                var p1 = points[i];
                var p2 = points[i + 1];
                UnityEngine.Debug.DrawLine(p1, p2, color);
            }

            UnityEngine.Debug.DrawLine(points[len - 1], points[0], color);
        }
        static public void _DrawAll(Vector2[] left, Vector2[] right, UnityEngine.Vector3 center, UnityEngine.Color color)
        {
            
            for (int i = 0; i < left.Length - 1; i++)
            {
                var pos1 = center + new UnityEngine.Vector3(left[i].X,0, left[i].Y);
                var pos2 = center + new UnityEngine.Vector3(left[i + 1].X, 0, left[i + 1].Y);
                UnityEngine.Debug.DrawLine(pos1, pos2, color);
            }

            for (int i = 0; i < right.Length - 1; i++)
            {
                var pos1 = center + new UnityEngine.Vector3(right[i].X, 0, right[i].Y);
                var pos2 = center + new UnityEngine.Vector3(right[i + 1].X, 0, right[i + 1].Y);
                UnityEngine.Debug.DrawLine(pos1, pos2, color);
            }

        }
#endif

        #endregion


        void IStage.Update()
        {
            var nowTime = _CastTimer.Second;
            var poly = _Caster.FindDetermination(_CurrentCastTime, nowTime);
            _CurrentCastTime = nowTime;


            bool guardImpact = false;
            _Player.SetBlock(false);
            if (poly != null)
            {
                if (_Caster.IsBlock())
                {
                    _Player.SetBlock(true);
                }

                var dir = -_Player.Direction * ((float)Math.PI / 180f);
                var center = _DatumPosition;
                if (_Caster.IsControll() )
                {
                    center = _Player.GetPosition();
                }
                
                poly.Rotation(dir, new Vector2());
                poly.Offset(center);

                #region UnityDebugCode
#if UNITY_EDITOR                
                _DrawAll( (from p in _Caster.Data.Lefts select Regulus.CustomType.Polygon.RotatePoint(p, new Vector2(), dir)).ToArray()
                    , (from p in _Caster.Data.Rights select Regulus.CustomType.Polygon.RotatePoint(p, new Vector2(), dir)).ToArray()
                    , new UnityEngine.Vector3(center.X , 0.5f , center.Y) , UnityEngine.Color.blue);
                _Draw(poly ,0.5f , UnityEngine.Color.green);
#endif
                #endregion

                var results = _Map.Find(poly.Points.ToRect());

                foreach (var individual in results)
                {
                    if (individual.Id == _Player.Id)
                        continue;
                    var collision = Regulus.CustomType.Polygon.Collision(poly, individual.Mesh, new Vector2());
                    if (collision.Intersect)
                    {
                        if (_Caster.IsSmash())
                        {
                            _AttachDamage(individual, true);
                        }
                        else if (individual.IsBlock() == false && _Caster.IsPunch())
                        {
                            _AttachDamage(individual, false);
                        }
                        else if (individual.IsBlock() && _Caster.IsPunch())
                        {
                            guardImpact = true;
                        }
                    }
                }
            }


            if (guardImpact)
                NextEvent(SkillCaster.Build(ACTOR_STATUS_TYPE.GUARD_IMPACT));
            else if (_Caster.IsDone(_CurrentCastTime))
            {
                
                if (_Overdraft)
                {
                    NextEvent(SkillCaster.Build(ACTOR_STATUS_TYPE.TIRED));
                }
                else
                {
                    BattleIdleEvent();
                }
            }
                
        }

        private void _AttachDamage(IIndividual target, bool smash)
        {
            if (_Attacked.Contains(target.Id) == false)
            {
                var hitForce = new HitForce();
                hitForce.Damage = smash
                    ? 3.0f
                    : 1f;
                target.AttachDamage(_Player.Id , hitForce);
                _Attacked.Add(target.Id);
            }
        }

        ACTOR_STATUS_TYPE ICastSkill.Id { get { return _Caster.Data.Id; } }

        ACTOR_STATUS_TYPE[] ICastSkill.Skills
        {
            get { return _Caster.Data.Nexts; }
        }

        void ICastSkill.Cast(ACTOR_STATUS_TYPE skill)
        {
            if (_Overdraft)
                return;
            var caster = _Caster.FindNext(skill);
            if (caster != null)
            {
                NextEvent(caster);
            }
        }

        void IBattleSkill.Disarm()
        {
            DisarmEvent();
        }
    }
}