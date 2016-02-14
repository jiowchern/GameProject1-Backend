using System;
using System.Collections.Generic;

using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class ActorMemory
    {
        private readonly Dictionary<Guid, Actor> _Actors;

        public ActorMemory()
        {
            _Actors = new Dictionary<Guid, Actor>();
        }

        public void Add(Guid id)
        {
            if(_Actors.ContainsKey(id) == false)
                _Actors.Add(id , new Actor(id));
        }

        public bool Have(Guid id)
        {
            return _Actors.ContainsKey(id);
        }

        public void Forget(float delta)
        {
            List<Guid> removes = new List<Guid>();

            foreach (var keyPair in _Actors)
            {
                var actor = keyPair.Value;
                if (actor.TimeUp(delta))
                {
                    removes.Add(actor.Id);
                }
            }
            foreach (var remove in removes)
            {
                _Actors.Remove(remove);
            }
            
        }

        internal class Actor
        {
            private readonly Guid _Id;

            private float _Durability;

            public Actor(Guid id)
            {
                _Id = id;
                _Durability = 60.0f;
            }

            public Guid Id { get { return _Id; } }

            public bool TimeUp(float delta)
            {
                _Durability -= delta;
                return _Durability > 0;
            }
        }

        public IVisible FindEnemy(List<IVisible> vision)
        {
            return null;
        }
    }

    
}