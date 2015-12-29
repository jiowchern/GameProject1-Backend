using System;
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

        public event Action DoneEvent;

        private Guid _Id;

        public BattleCasterStatus(ISoulBinder binder, Entity player, Map map, SkillCaster caster)
        {
            _Id = Guid.NewGuid();
            _Binder = binder;
            _Player = player;
            _Map = map;
            _Caster = caster;            
        }

        void IStage.Enter()
        {
            _Player.CastBegin(_Caster.Data.Id);
        }

        void IStage.Leave()
        {
            _Player.CastEnd(_Caster.Data.Id);
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
                        individual.AttachDamage(_Id, _Caster);
                    }
                }
            }
            
            

            if (_Caster.IsDone())
                DoneEvent();
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