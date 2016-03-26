using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

using Regulus.CustomType;
using Regulus.Framework;
using Regulus.Utility;
using Regulus.Project.ItIsNotAGame1.Data;
namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Zone :IUpdatable
    {
        

        private readonly Dictionary<string, Realm> _Realms;

        public Zone(RealmInfomation[] realm_infomations)
        {
            if (realm_infomations == null)
                throw new System.NullReferenceException();

            this._Realms = new Dictionary<string, Realm>();

            foreach (var material in realm_infomations)
            {
                this._Realms.Add(material.Name , new Realm(material.Dimension));
            }            
        }

        public Realm FindRealm(string name)
        {
            Realm realm;
            return this._Realms.TryGetValue(name, out realm)
                ? realm
                : null;
        }
        public Map FindMap(string name)
        {
            Realm realm;
            return this._Realms.TryGetValue(name, out realm)
                ? realm.Query()
                : null;
        }
        
        void IBootable.Launch()
        {
            
        }

        
        void IBootable.Shutdown()
        {
            
        }

        bool IUpdatable.Update()
        {
            return true;
        }
    }
}