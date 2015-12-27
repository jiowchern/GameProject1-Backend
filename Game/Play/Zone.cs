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
        

        private Dictionary<string, Realm> _Realms;

        public Zone(RealmMaterial[] realm_materials)
        {
            if (realm_materials == null)
                throw new System.NullReferenceException();

            this._Realms = new Dictionary<string, Realm>();

            foreach (var material in realm_materials)
            {
                this._Realms.Add(material.Name , new Realm());
            }            
        }
        
        public Map FindMap(string name)
        {
            Realm map = null;
            return this._Realms.TryGetValue(name, out map)
                ? map.Query()
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