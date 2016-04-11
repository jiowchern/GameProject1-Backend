using System.Linq;

using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class EnterancePostProduction
    {
        

        private readonly IMapFinder _Finder;

        private readonly ENTITY[] _Types;

        public EnterancePostProduction(IMapFinder finder, ENTITY[] types)
        {        
            _Finder = finder;
            _Types = types;
        }

        public IUpdatable ProcessStronghold(Entity[] entitys,IMapGate gate)
        {

            var owner = (from e in entitys where e.Type == ENTITY.ENTRANCE select e).Single();
            return new InorganicsOwner(entitys , gate , new StrongholdWisdom(_Types , owner , gate , _Finder));
        }

        public IUpdatable ProcessEnterance(Entity[] entitys, IMapGate gate)
        {
            var owner = (from e in entitys where e.Type == ENTITY.ENTRANCE select e).Single();
            return new InorganicsOwner(entitys, gate, new EnteranceWisdom(_Types, owner, gate));
        }

        public IUpdatable ProcessField(Entity[] entitys, IMapGate gate)
        {
            var owner = (from e in entitys where e.Type == ENTITY.FIELD select e).Single();
            return new InorganicsOwner(entitys, gate, new FieldWisdom(_Types, owner, gate, _Finder));
        }
    }
}