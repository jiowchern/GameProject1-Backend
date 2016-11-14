using System;
using System.Security.Policy;

using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class RealmReadyStage : IStage , IJumpMap
    {
        private readonly ISoulBinder _Binder;

        private readonly Zone _Zone;

        private readonly string _Target;

        private Realm.Map _Map;

        public event Action<Realm.Map> GameEvent;
        public event Action ErrorEvent;

        public RealmReadyStage(ISoulBinder binder, Zone zone, string target)
        {
            _Binder = binder;
            _Zone = zone;
            _Target = target;
        }

        void IStage.Enter()
        {
            var realm = this._Zone.FindRealm(_Target);
            if (realm != null)
            {
                var mapResult = realm.QueryMap();
                mapResult.OnValue += _GetMap;
            }
            else
            {
                ErrorEvent();
            }
        }

        private void _GetMap(Realm.Map obj)
        {
            _Map = obj;
            _Binder.Bind<IJumpMap>(this);
        }

        void IStage.Leave()
        {
            _Binder.Unbind<IJumpMap>(this);
        }

        void IStage.Update()
        {
            
        }

        string IJumpMap.Realm
        {
            get { return _Target; }
        }

        void IJumpMap.Ready()
        {
            GameEvent(_Map);
        }
    }
}