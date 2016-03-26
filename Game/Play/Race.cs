using System;
using System.Collections.Generic;
using System.Linq;

using Regulus.CustomType;
using Regulus.Framework;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class Race :Regulus.Utility.IUpdatable , IMapEntityProivder
    {
        private readonly Zone _Zone;

        

        private readonly TimesharingUpdater _Updater;

        private readonly IRandom _Random;

        public Race(Zone zone)
        {
            _Random = Regulus.Utility.Random.Instance;
            this._Zone = zone;
            _Updater = new TimesharingUpdater(1.0f / 30.0f);

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
            return new Aboriginal(map, entiry, wisdom);
        }

        void IBootable.Launch()
        {
            var realm = _Zone.FindRealm("test");                        
            var map = realm.NewMap(this);            

            _CreateChallengers(map);

        }

        

        

        void IBootable.Shutdown()
        {
            _Updater.Shutdown();
        }

        bool IUpdatable.Update()
        {
            _Updater.Working();
            return true;
        }

        IEnumerable<IIndividual> IMapEntityProivder.NewRoom(Vector2 center, float direction)
        {
            return _BuildEntityGroup("room", direction, center , _CreateEntity );
        }

        

        IEnumerable<IIndividual> IMapEntityProivder.NewEnterance(Vector2 center, float direction)
        {
            return _BuildEntityGroup("enterance", direction, center , _CreateEnterance );
        }

        

        IEnumerable<IIndividual> IMapEntityProivder.NewResource(Vector2 center, float direction)
        {
            return _BuildEntityGroup("pool", direction, center , _CreateResource);
        }

        void _CreateChallengers(Map map)
        {
            var itemProvider = new ItemProvider();
            
            for (int i = 0; i < 50; i++)
            {
                var aboriginal = _Create(map, ENTITY.ACTOR2, itemProvider);
            
                _Updater.Add(aboriginal);
                
            }

            for (int i = 0; i < 50; i++)
            {
                var aboriginal = _Create(map, ENTITY.ACTOR3, itemProvider);
            
                _Updater.Add(aboriginal);
            }

            for (int i = 0; i < 50; i++)
            {
                var aboriginal = _Create(map, ENTITY.ACTOR4, itemProvider);
            
                _Updater.Add(aboriginal);
            }

            for (int i = 0; i < 50; i++)
            {
                var aboriginal = _Create(map, ENTITY.ACTOR5, itemProvider);
            
                _Updater.Add(aboriginal);
            }

            
        }

        IEnumerable<IIndividual> IMapEntityProivder.NewWall(Vector2 center, float direction)
        {
            return _BuildEntityGroup("wall", direction, center , _CreateEntity);
        }


        private IEnumerable<IIndividual> _BuildEntityGroup(string id, float degree, Vector2 center , Func<ENTITY, IIndividual> indinidual_provider)
        {

            var layout = Resource.Instance.FindEntityGroupLayout(id);
            var buildInfos = from e in layout.Entitys
                             let radians = degree * (float)System.Math.PI / 180f
                             let position = Polygon.RotatePoint(e.Position, new Vector2(), radians)
                             select new
                             {
                                 EntityType = e.Type,
                                 Position = position + center,
                                 Direction = e.Direction + degree
                             };

            List<IIndividual> individuals = new List<IIndividual>();
            foreach (var info in buildInfos)
            {
                IIndividual individual = indinidual_provider(info.EntityType);
                individual.SetPosition(info.Position);
                individual.AddDirection(degree);
                individuals.Add(individual);
            }
            return individuals;
        }
        private IIndividual _CreateEntity(ENTITY type)
        {
            return EntityProvider.Create(type);
        }

        private IIndividual _CreateResource(ENTITY type)
        {
            return EntityProvider.CreateResource(type, new ResourceInventory());
        }

        private IIndividual _CreateEnterance(ENTITY type)
        {
            if (type == ENTITY.ENTRANCE)
            {
                ENTITY[] types = new ENTITY[0];
                var val = _Random.NextInt(0, 5);
                if (val == 0)
                {
                    types = new ENTITY[] { ENTITY.ACTOR2, ENTITY.ACTOR1 };
                }
                else if (val == 1)
                {
                    types = new ENTITY[] { ENTITY.ACTOR2, ENTITY.ACTOR3 };
                }
                else if (val == 2)
                {
                    types = new ENTITY[] { ENTITY.ACTOR2, ENTITY.ACTOR4 };
                }
                else if (val == 3)
                {
                    types = new ENTITY[] { ENTITY.ACTOR2, ENTITY.ACTOR5 };
                }
                return EntityProvider.CreateEnterance(types);
            }
            else
            {
                return EntityProvider.Create(type);
            }
        }
    }
}
