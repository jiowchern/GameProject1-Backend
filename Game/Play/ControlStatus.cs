using System;

using Regulus.Framework;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class ControlStatus : Regulus.Utility.IUpdatable , Data.IActorStatus
    {
        private readonly ISoulBinder _Binder;

        private readonly Entity _Player;

        private readonly Mover _Mover;

        private readonly Map _Map;

        private readonly StageMachine _Status;

        private ACTOR_STATUS_TYPE _Current;

        public ControlStatus(ISoulBinder binder, Entity player, Mover mover , Map map)
        {
            _Binder = binder;
            _Player = player;
            _Mover = mover;
            _Map = map;
            _Status = new StageMachine();
        }

        void IBootable.Launch()
        {
            _Binder.Bind<Data.IActorStatus>(this);
            _ToNormal();
        }

        private void _ToNormal()
        {            
            var status = new NormalStatus(_Binder, _Player);
            status.ExploreEvent += _ToExplore; 
            _SetStatus(status);
        }

        private void _SetStatus(ActorStatus status)
        {
            _Change(status.ActorStatusType);
            _Status.Push(status);
        }

        private void _ToExplore(Guid obj)
        {
            var status = new ExploreStatus(_Binder, _Player , _Map , obj);
            status.DoneEvent += _ToNormal;
            _SetStatus(status);
        }

        void IBootable.Shutdown()
        {
            _Status.Termination();
            _Binder.Unbind<Data.IActorStatus>(this);
        }

        bool IUpdatable.Update()
        {
            _Status.Update();
            return true;
        }

        private event Action<ACTOR_STATUS_TYPE, ACTOR_STATUS_TYPE> _OnChangeEvent;

        Guid IActorStatus.Id {
            get { return _Player.Id; }
        }

        event Action<ACTOR_STATUS_TYPE, ACTOR_STATUS_TYPE> IActorStatus.ChangeEvent
        {
            add { this._OnChangeEvent += value; }
            remove { this._OnChangeEvent -= value; }
        }

        protected void _Change(ACTOR_STATUS_TYPE arg1)
        {            
            if(_OnChangeEvent != null)
                _OnChangeEvent.Invoke(arg1, _Current);
            _Current = arg1;
        }
    }
}