using System;
using System.Collections.Generic;
using System.Linq;

using Regulus.CustomType;
using Regulus.Extension;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;



using Vector2 = Regulus.CustomType.Vector2;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class BattleCasterStatus : IStage, ICastSkill
    {
        private readonly ISoulBinder _Binder;

        private readonly Entity _Player;

        private readonly Map _Map;

        private readonly SkillCaster _Caster;

        public event Action<SkillCaster> NextEvent;
        public event Action DoneEvent;

        public HashSet<Guid> _Attacked;

        private Regulus.Utility.TimeCounter _CastTimer;

        private float _CurrentCastTime;
        public BattleCasterStatus(ISoulBinder binder, Entity player, Map map, SkillCaster caster)
        {
            _CastTimer = new TimeCounter();
            _Attacked = new HashSet<Guid>();
            _Binder = binder;
            _Player = player;
            _Map = map;
            _Caster = caster;
        }

        void IStage.Enter()
        {

            _Player.CastBegin(_Caster.Data.Id);
            _CastTimer.Reset();
            if (_Caster.IsBlock())
            {
                _Player.SetBlock(true);
            }
        }

        void IStage.Leave()
        {
            _Player.CastEnd(_Caster.Data.Id);
            if (_Caster.IsBlock())
            {
                _Player.SetBlock(false);
            }
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
            Regulus.CustomType.Polygon poly = _Caster.FindDetermination(_CurrentCastTime, nowTime);
            _CurrentCastTime = nowTime;

            
            bool guardImpact = false;
            if (poly != null)
            {
                var dir = -_Player.Direction * ((float)Math.PI / 180f);

                var center = _Player.GetPosition();
                poly.Rotation(dir, new Vector2());
                poly.Offset(center);


                #region UnityDebugCode
#if UNITY_EDITOR
                UnityEngine.Debug.Log("_Player.Direction : " + _Player.Direction);
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
                DoneEvent();
        }

        private void _AttachDamage(IIndividual target, bool smash)
        {
            if (_Attacked.Contains(target.Id) == false)
            {
                target.AttachDamage(smash);
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

        }
    }
}