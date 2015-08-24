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
    public class Zone :Regulus.Utility.IUpdatable
    {
        class Realm
        {
            private readonly EntityData[] _EntityDatas;
            private readonly Map _Map;

            public Realm(EntityData[] entity_datas)
            {
                _EntityDatas = entity_datas;
                _Map = new Map();
            }

            public Map Query()
            {
                return _Map;
            }
        }
        private Dictionary<string, Realm> _Realms;

        

        public Zone(RealmMaterial[] realm_materials)
        {
            if (realm_materials == null)
                throw new System.NullReferenceException();

            _Realms = new Dictionary<string, Realm>();

            foreach (var material in realm_materials)
            {
                _Realms.Add(material.Name , new Realm(material.EntityDatas));
            }            
        }
        
        public Map FindMap(string name)
        {
            Realm map = null;
            return _Realms.TryGetValue(name, out map)
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