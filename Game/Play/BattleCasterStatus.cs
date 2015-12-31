using System;
using System.Collections.Generic;
using System.Linq;

using Regulus.CustomType;
using Regulus.Extension;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class BattleCasterStatus : IStage , ICastSkill
    {
        private readonly ISoulBinder _Binder;

        private readonly Entity _Player;

        private readonly Map _Map;

        private readonly SkillCaster _Caster;

        public event Action<SkillCaster> NextEvent;
        public event Action DoneEvent;

        public HashSet<Guid> _Attacked;

        public BattleCasterStatus(ISoulBinder binder, Entity player, Map map, SkillCaster caster)
        {
            _Attacked = new HashSet<Guid>();
            _Binder = binder;
            _Player = player;
            _Map = map;
            _Caster = caster;            
        }

        void IStage.Enter()
        {
            _Player.CastBegin(_Caster.Data.Id);

            if(_Caster.IsBlock())
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
        /*
        Unity Debug Code
        private void _Draw(Polygon result , Color color)
        {
            var points = (from p in result.Points select new UnityEngine.Vector3(p.X, 1, p.Y)).ToArray();
            var len = points.Length;
            if (len < 2)
            {
                return;
            }
            for (int i = 0; i < len - 1; i++)
            {
                var p1 = points[i];
                var p2 = points[i + 1];
                UnityEngine.Debug.DrawLine( p1, p2, color);
            }

            UnityEngine.Debug.DrawLine(points[len - 1], points[0], color);
        }*/
        void IStage.Update()
        {
            Regulus.CustomType.Polygon poly =  _Caster.Find();
            

            bool guardImpact = false;
            if (poly != null)
            {
                var center = _Player.GetPosition();
                poly.Rotation(_Player.Direction);                
                poly.Offset(center);


                var results = _Map.Find(poly.Points.ToRect());

                foreach (var individual in results)
                {
                    if(individual.Id == _Player.Id)
                        continue;
                    var collision = Regulus.CustomType.Polygon.Collision(poly, individual.Mesh, new Vector2());
                    if (collision.Intersect)
                    {
                        if(_Caster.IsSmash())
                        {
                            _AttachDamage(individual , true);                            
                        }
                        else if(individual.IsBlock() == false && _Caster.IsPunch())
                        {
                            _AttachDamage(individual , false);
                        }
                        else if (individual.IsBlock() && _Caster.IsPunch())
                        {
                            guardImpact = true;
                            
                            
                        }
                    }
                }
            }
            
            
            if(guardImpact)
                NextEvent(SkillCaster.Build(ACTOR_STATUS_TYPE.GUARD_IMPACT));
            else if (_Caster.IsDone())
                DoneEvent();
        }

        private void _AttachDamage(IIndividual target , bool smash)
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
            get { return _Caster.Data.Nexts;  }
        }

        void ICastSkill.Cast(ACTOR_STATUS_TYPE skill)
        {
            
        }
    }
}