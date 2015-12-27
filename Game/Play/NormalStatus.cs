using System;

using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class NormalStatus : ActorStatus , IMoveController , ISkillController
    {
        private readonly ISoulBinder _Binder;

        private readonly Entity _Player;

        public event Action<Guid> ExploreEvent;

        public NormalStatus(ISoulBinder binder, Entity player) : base (ACTOR_STATUS_TYPE.NORMAL )
        {
            _Binder = binder;
            _Player = player;
        }
        

        public override void Leave()
        {
            _Binder.Unbind<ISkillController>(this);
            _Binder.Unbind<IMoveController>(this);
        }

        public override void Update()
        {
            
        }

        public override void Enter()
        {
            _Binder.Bind<IMoveController>(this);
            _Binder.Bind<ISkillController>(this);
        }
        

        void IMoveController.Forward()
        {
            _Player.Move(0 , false);
        }

        void IMoveController.Backward()
        {
            _Player.Move(180, false);
        }

        void IMoveController.StopMove()
        {
            _Player.Stop();
        }

        void IMoveController.TrunLeft()
        {
            _Player.Trun(-300);
        }

        void IMoveController.TrunRight()
        {
            _Player.Trun(300);
        }

        void IMoveController.StopTrun()
        {
            _Player.Trun(0);
        }

        void IMoveController.RunForward()
        {
            _Player.Move(0, true);
        }

        void ISkillController.Explore(Guid target)
        {
            ExploreEvent(target);
        }
    }
}