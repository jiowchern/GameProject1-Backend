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

        
        
        private readonly List<IIndividual> _Damages;

        public BattleCasterStatus(ISoulBinder binder, Entity player, Map map, SkillCaster caster)
        {
            _Damages = new List<IIndividual>();        
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


            foreach(var damage in _Damages)
            {
                damage.AttachDamage();
            }
        }

        void IStage.Update()
        {
            Regulus.CustomType.Polygon poly =  _Caster.Find();
            if (poly != null)
            {
                var results = _Map.Find(poly.Points.ToRect());

                foreach (var individual in results)
                {
                    var collision = Regulus.CustomType.Polygon.Collision(poly, individual.Mesh, new Vector2());
                    if (collision.Intersect)
                    {
                        if(_Caster.IsSmash())
                        {
                            _AttachDamage(individual);
                            _AttachDamage(individual);
                            _AttachDamage(individual);
                        }
                        else if(individual.IsBlock() == false && _Caster.IsPunch())
                        {
                            _AttachDamage(individual);
                        }
                        else if (individual.IsBlock() && _Caster.IsPunch())
                        {                            
                            NextEvent(SkillCaster.Build(ACTOR_STATUS_TYPE.GUARD_IMPACT));
                            return;
                        }
                    }
                }
            }
            
            

            if (_Caster.IsDone())
                DoneEvent();
        }

        private void _AttachDamage(IIndividual target)
        {
            _Damages.Add(target);
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