using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

using Regulus.CustomType;
using Regulus.Framework;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class Race :Regulus.Utility.IUpdatable , IMapGate
    {
        private readonly Zone _Zone;

        private readonly Map _Map;


        private readonly Dictionary<ENTITY, int> _EntityResource;
        private readonly Dictionary<ENTITY, int> _EntityFieldResource;
        private readonly Dictionary<LEVEL_UNIT, EntityGroupBuilder> _LevelUnitToGroupBuilder;
        private readonly TimesharingUpdater _Updater;

        private readonly IRandom _Random;

        private readonly List<Entity> _WaitingRoom;

        private readonly List<Entity> _Inorganics;

        public Race(Zone zone)
        {
            _EntityResource = new Dictionary<ENTITY, int>
            {
                { ENTITY.ACTOR1, 50},
                { ENTITY.ACTOR2, 50},
                { ENTITY.ACTOR3, 50},
                { ENTITY.ACTOR4, 50},
                { ENTITY.ACTOR5, 50},
            };

            _EntityFieldResource = new Dictionary<ENTITY, int>
            {
                { ENTITY.ACTOR1, 50},
                { ENTITY.ACTOR2, 50},
                { ENTITY.ACTOR3, 50},
                { ENTITY.ACTOR4, 50},
                { ENTITY.ACTOR5, 50},
            };
            _Inorganics = new List<Entity>();
            _WaitingRoom = new List<Entity>();
            _Map = new Map();

            _LevelUnitToGroupBuilder = new Dictionary<LEVEL_UNIT, EntityGroupBuilder>
            {
                {LEVEL_UNIT.WALL, new EntityGroupBuilder("wall", _AllInorganics )},
                {LEVEL_UNIT.ENTERANCE1, new EntityGroupBuilder("enterance", new EnterancePostProduction(_Map , new [] {ENTITY.ACTOR1}).ProcessEnterance )},
                {LEVEL_UNIT.ENTERANCE2, new EntityGroupBuilder("enterance", new EnterancePostProduction(_Map , new [] {ENTITY.ACTOR3, ENTITY.ACTOR2 }).ProcessStronghold )},
                {LEVEL_UNIT.ENTERANCE3, new EntityGroupBuilder("enterance", new EnterancePostProduction(_Map , new [] {ENTITY.ACTOR4, ENTITY.ACTOR2 }).ProcessStronghold )},
                {LEVEL_UNIT.ENTERANCE4, new EntityGroupBuilder("enterance", new EnterancePostProduction(_Map , new [] {ENTITY.ACTOR5, ENTITY.ACTOR2 }).ProcessStronghold )},
                {LEVEL_UNIT.CHEST, new EntityGroupBuilder("chest", _AllInorganics )},
                {LEVEL_UNIT.FIELD, new EntityGroupBuilder("field", new EnterancePostProduction(_Map , new [] {ENTITY.ACTOR3 , ENTITY.ACTOR4 , ENTITY.ACTOR5 }).ProcessField ) },
                {LEVEL_UNIT.GATE, new EntityGroupBuilder("thickwall", _AllInorganics )},
                {LEVEL_UNIT.POOL, new EntityGroupBuilder("pool", _ResourcePostProduction )},                
            };
            _Random = Regulus.Utility.Random.Instance;
            this._Zone = zone;
            _Updater = new TimesharingUpdater(1.0f / 30.0f);

            
        }

        private IUpdatable _AllInorganics(Entity[] entitys, IMapGate gate)
        {
            _Inorganics.AddRange(entitys);
            return null;
        }

        private IUpdatable _ResourcePostProduction(Entity[] entitys, IMapGate gate)
        {

            foreach (var entity in entitys)
            {
                entity.SetBag( new ResourceBag());
            }
            _Inorganics.AddRange(entitys);

            return null;
        }

        

        private Aboriginal _Create(Map map,ENTITY type, ItemProvider item_provider)
        {
            
            var entiry = EntityProvider.Create(type);
            var items = item_provider.FromStolen();
            foreach (var item in items)
            {
                entiry.Bag.Add(item);
            }            
            var wisdom = new StandardWisdom(entiry);
            var aboriginal = new Aboriginal(map, this, entiry, wisdom);            
            return aboriginal;
        }

        void IBootable.Launch()
        {
            var realm = _Zone.FindRealm("test");                        
            var level = realm.CreateLevel(this,_Map);
            
            foreach (var unit in level)
            {
                var builder = _LevelUnitToGroupBuilder[unit.Type];                
                var updater = builder.Create(unit.Direction, unit.Center , this );                
                if(updater != null)
                    _Join(updater);
            }
            foreach (var inorganic in _Inorganics)
            {
                _Map.JoinStaff(inorganic);
            }
            
        }

        private void _Join(IUpdatable aboriginal)
        {
            _Updater.Add(aboriginal);
        }

        void IBootable.Shutdown()
        {

            foreach (var inorganic in _Inorganics)
            {
                _Map.Left(inorganic);
            }

            _Updater.Shutdown();
        }

        bool IUpdatable.Update()
        {
            _Updater.Working();
            return true;
        }


        void IMapGate.Left(Entity player)
        {
            _Map.Left(player);
            _WaitingRoom.RemoveAll((e) => e.Id == player.Id);
        }

        void IMapGate.Join(Entity player)
        {
            if(_InWaitingRoom(player))
                _WaitingRoom.Add(player);            
            else
            {
                _Map.JoinStaff(player);
            }
        }

        private bool _InWaitingRoom(Entity player)
        {
            if (EntityData.IsActor(player.Type))
            {                
                return true;
            }
            return false;
        }

        void IMapGate.Pass(Vector2 position, ENTITY[] types)
        {
            var entity = (from e in _WaitingRoom where types.Any( t => t == e.Type) select e).FirstOrDefault();
            if (entity != null)
            {
                IIndividual individual = entity;
                _WaitingRoom.RemoveAll((e) => e.Id == entity.Id);
                individual.SetPosition(position);
                _Map.JoinStaff(entity);
            }
                
        }

        void IMapGate.Pass(Vector2 position, Guid id)
        {
            var entity = (from e in _WaitingRoom where e.Id == id select e).FirstOrDefault();
            if (entity != null)
            {
                IIndividual individual = entity;
                _WaitingRoom.RemoveAll((e) => e.Id == entity.Id);
                individual.SetPosition(position);
                _Map.JoinStaff(entity);
            }
        }

        Guid IMapGate.Spawn(ENTITY type)
        {
            if (_EntityResource[type] > 0)
            {
                var itemProvider = new ItemProvider();
                var aboriginal = _Create(_Map, type, itemProvider);
                _EntityResource[type]--;
                _Updater.Add(aboriginal);
                aboriginal.DoneEvent += () =>
                {
                    _EntityResource[type]++;
                    _Updater.Remove(aboriginal);
                };
                return aboriginal.Entity.Id;
            }
            return Guid.Empty;
        }

        Guid[] IMapGate.SpawnField(ENTITY[] types)
        {
            foreach (var type in types)
            {
                if(_EntityFieldResource[type] <= 0)
                    return new Guid[0];
            }
            List<Guid> ids = new List<Guid>();
            foreach (var type in types)
            {
                if (_EntityFieldResource[type] > 0)
                {
                    var itemProvider = new ItemProvider();
                    var aboriginal = _Create(_Map, type, itemProvider);
                    _EntityFieldResource[type]--;
                    _Updater.Add(aboriginal);
                    aboriginal.DoneEvent += () =>
                    {
                        _EntityFieldResource[type]++;
                        _Updater.Remove(aboriginal);
                    };
                    ids.Add(aboriginal.Entity.Id);
                }
            }
            return ids.ToArray();
        }
    }
}
