using System;
using System.Linq;

using Regulus.CustomType;
using Regulus.Extension;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class ExploreStatus : ActorStatus 
    {
        private readonly Entity _Player;

        private readonly Map _Map;

        private readonly Guid _TargetId;

        private readonly ISoulBinder _Binder;

        private readonly TimeCounter _CastTimer;

        public event Action DoneEvent;

        public ExploreStatus(ISoulBinder binder, Entity player , Map map , Guid target_id) : base(ACTOR_STATUS_TYPE.EXPLORE)
        {
            _Binder = binder;
            _Player = player;
            _Map = map;
            _TargetId = target_id;
            _CastTimer = new TimeCounter();
        }

        

        public override void Leave()
        {
            var explore = _Player.GetExploreBound();
            var results = _Map.Find(explore.Points.ToRect());
            var target = (from indivude in results where indivude.Id == _TargetId select indivude).SingleOrDefault();
            if (target != null)
            {
                var result = Polygon.Collision(target.Mesh, explore, new Vector2());
                if (result.Intersect)
                {
                    var items = target.Stolen();
                    foreach (var item in items)
                    {
                        _Player.Bag.Add(item);
                    }

                }
            }
        }

        public override void Update()
        {
            if (_CastTimer.Second > 1.0f)
            {
                DoneEvent();
            }
        }

        public override void Enter()
        {
            
        }

        

        
    }
}