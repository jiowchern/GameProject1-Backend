using System;
using System.Collections.Generic;

using Regulus.CustomType;

using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Realm
    {        
        private Map _Map;

        private IMapGate _Gate;

        private readonly float _Witdh;

        private readonly float _Height;


        private readonly int _Dimension;

        
        public Realm(int dimension)
        {
            _Dimension = dimension;

            this._Witdh = 15    ;
            this._Height = 15;

        }
      
                

        public IMapFinder GetFinder()
        {
            return this._Map;
        }

        public IMapGate GetGate()
        {
            return this._Gate;
        }

        public Level CreateLevel(IMapGate gate , Map map)
        {
            _Map = map;
            _Gate = gate;
            var builder = new LevelGenerator(_Witdh, _Height);
            var level = builder.Build(_Dimension);            
            return level;
        }
    }
}