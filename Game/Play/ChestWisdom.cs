using System;
using System.Linq;
using System.Linq.Expressions;

using Regulus.BehaviourTree;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class ChestWisdom : Wisdom , ITicker
    {
        private enum MODE
        {
            IDLE,

            EXCLUDE
        };

        private MODE _Mode;
        private readonly ENTITY[] _Types;

        private readonly IMapGate _Gate;

        private Entity _Owner;
        

        private readonly IMapFinder _Finder;

        private Entity _Exit;

        private Entity _Door;

        private Entity _Chest;

        private Entity[] _Walls;

        private Regulus.Utility.StageMachine _Machine;

        public ChestWisdom(ENTITY[] types, Entity[] entitys, IMapGate gate, IMapFinder finder)
        {
            _Machine = new StageMachine();
            _Types = types;
            _Gate = gate;
            _Finder = finder;

            _Owner = (from e in entitys where e.Type == ENTITY.CHEST_OWNER select e).FirstOrDefault();
            if(_Owner == null)
                throw new Exception("Can not find the owner.");            

            _Exit = (from e in entitys where e.Type == ENTITY.CHEST_EXIT select e).FirstOrDefault();
            if (_Exit == null)
                throw new Exception("Can not find the exit.");

            _Chest = (from e in entitys where e.Type == ENTITY.DEBIRS select e).FirstOrDefault();
            if (_Chest == null)
                throw new Exception("Can not find the chest.");

            _Door = (from e in entitys where e.Type == ENTITY.CHEST_GATE select e).FirstOrDefault();
            if (_Door == null)
                throw new Exception("Can not find the door.");

            _Walls = (from e in entitys where e.Type == ENTITY.WALL_GATE select e).ToArray();
            
        }

        

        

        protected override void _Update(float delta)
        {
            
        }

        protected override void _Shutdown()
        {
            foreach (var wall in _Walls)
            {
                _Gate.Left(wall);
            }
            _Machine.Termination();
            
        }

        protected override ITicker _Launch()
        {
            foreach (var wall in _Walls)
            {
                _Gate.Join(wall);
            }

            var builder = new Regulus.BehaviourTree.Builder();
            var ticker = builder
                    .Sequence()
                        .Action(() => new ChestIdleAction(_Chest, _Gate))
                        .Action(() => new ChestExcludeAction(_Gate , _Finder, _Owner , _Door  , _Exit ))
                    .End()
                .Build();
            return ticker;
        }

        

        void ITicker.Reset()
        {
            
        }

        TICKRESULT ITicker.Tick(float delta)
        {
            return TICKRESULT.SUCCESS;
        }
    }
}
