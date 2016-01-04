using System;

using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class NormalStatus : ActorStatus , INormalSkill
    {
        private readonly ISoulBinder _Binder;

        private readonly Entity _Player;

        public event Action<Guid> ExploreEvent;

        public event Action BattleEvent;
        public event Action MakeEvent;

        private readonly MoveController _MoveController;

        public NormalStatus(ISoulBinder binder, Entity player) 
        {
            _Binder = binder;
            _Player = player;

            _MoveController = new MoveController(_Player);
        }
        

        public override void Leave()
        {
            _Binder.Unbind<INormalSkill>(this);
            _Binder.Unbind<IMoveController>(_MoveController);
        }

        public override void Update()
        {
            
        }

        public override void Enter()
        {


            _Binder.Bind<IMoveController>(_MoveController);
            _Binder.Bind<INormalSkill>(this);

            _Player.Normal();
        }
        

        

        void INormalSkill.Explore(Guid target)
        {
            ExploreEvent(target);
        }

        public void Battle()
        {
            BattleEvent();
        }

        void INormalSkill.Make()
        {
            MakeEvent();
        }
    }
}