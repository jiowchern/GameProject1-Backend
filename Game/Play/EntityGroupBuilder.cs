using System;
using System.Collections.Generic;
using System.Linq;
using Regulus.CustomType;
using Regulus.Framework;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class EntityGroupBuilder
    {
        private readonly string _Id;
        private readonly IMapFinder _Finder;
        private readonly IMapGate _Gate;


        internal EntityGroupBuilder(string id , IMapFinder finder , IMapGate gate)
        {
            _Id = id;
            _Finder = finder;
            _Gate = gate;
        }

        internal IEnumerable<IUpdatable> Create(float degree, Vector2 center , IMapGate gate, IMapFinder finder)
        {            
            return _CreateFromGroup(_Id, degree, center);            
        }        
        
        private IEnumerable<IUpdatable> _CreateFromGroup(string id, float degree, Vector2 center)
        {
            
            var layout = Resource.Instance.FindEntityGroupLayout(id);
            var buildInfos = from e in layout.Entitys
                        let radians = degree * (float) System.Math.PI/180f
                        let position = Polygon.RotatePoint(e.Position, new Vector2(), radians)
                        select new EntityCreateParameter
                        {
                            Id = e.Id,
                            Entity = EntityProvider.Create(e.Type , position + center , e.Direction + degree)                            
                        };

            foreach (var updatable in _CreateChests(layout.Chests, buildInfos))
            {
                yield return updatable;
            }

            foreach (var updatable in _StaticChests(layout.Statics, buildInfos))
            {
                yield return updatable;
            }
        }

        private IEnumerable<IUpdatable> _StaticChests(StaticLayout[] statics, IEnumerable<EntityCreateParameter> build_infos)
        {
            foreach (var layout in statics)
            {
                var owner = _Find(build_infos, layout.Owner);
                var chest = new StaticWisdom(owner);
                yield return chest;
            }
        }

        private IEnumerable<IUpdatable> _CreateChests(ChestLayout[] chests, IEnumerable<EntityCreateParameter> build_infos)
        {
            foreach (var chestLayout in chests)
            {
                var owner = _Find(build_infos, chestLayout.Owner);
                var exit = _Find(build_infos, chestLayout.Exit);
                var debirs = _Find(build_infos, chestLayout.Debirs);
                var gate = _Find(build_infos, chestLayout.Gate);

                var chest = new ChestWisdom(owner , exit , debirs , gate , _Finder , _Gate);
                yield return chest;
            }
        }

        private Entity _Find(IEnumerable<EntityCreateParameter> build_infos, Guid owner)
        {
            return (from build_info in build_infos where build_info.Id == owner select build_info.Entity).Single();
        }
    }

    internal class StaticWisdom : IUpdatable
    {
        public StaticWisdom(Entity owner)
        {
            throw new NotImplementedException();
        }

        void IBootable.Launch()
        {
            throw new NotImplementedException();
        }

        void IBootable.Shutdown()
        {
            throw new NotImplementedException();
        }

        bool IUpdatable.Update()
        {
            throw new NotImplementedException();
        }
    }
}