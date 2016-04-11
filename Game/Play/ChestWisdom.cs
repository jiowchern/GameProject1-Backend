using System;

using Regulus.BehaviourTree;
using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class ChestWisdom : Wisdom
    {
        private Entity _Entity;

        private ActorMind _Mind;
        private IMapFinder _Finder;
        public ChestWisdom(Entity entity , IMapFinder finder)
        {
            _Finder = finder;
            _Entity = entity;
        }

        protected override ITicker _Launch()
        {
            _BindGpi();

            return null;
        }

        

        protected override void _Update(float delta)
        {
            
        }

        protected override void _Shutdown()
        {
            _UnbindGpi();
        }

        private void _BindGpi()
        {
            
        }

        private void _UnbindGpi()
        {
            throw new NotImplementedException();
        }
    }
}