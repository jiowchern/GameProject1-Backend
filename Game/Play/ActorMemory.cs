using System;
using System.Collections.Generic;
using System.Linq;

using Regulus.Project.ItIsNotAGame1.Data;



namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class ActorMemory
    {
        private readonly ENTITY _EntityType;

        internal class Actor
        {
            private readonly Guid _Id;

            private float _Durability;

            private float _Imperil;


            public Actor(Guid id)
            {
                _Id = id;
                _Durability = 60.0f;
            }

            public Guid Id { get { return _Id; } }

            public float Imperil { get { return _Imperil; } }

            

            public bool TimeUp(float delta)
            {                
                _Durability -= delta;
                return _Durability < 0;
            }

            public void Hate(float damage)
            {
                _Durability += 10.0f;
                _Imperil += damage;
            }
        }

        private readonly Dictionary<Guid, Actor> _Actors;

        private Dictionary<ENTITY, ENTITY[]> _Enemys;
        public ActorMemory(ENTITY entity_type)
        {
            _EntityType = entity_type;
            _Actors = new Dictionary<Guid, Actor>();

            _Enemys = new Dictionary<ENTITY, ENTITY[]>
            {
                {
                    ENTITY.ACTOR3, new ENTITY[]
                    {
                        ENTITY.ACTOR4
                    }
                },
                {
                    ENTITY.ACTOR4, new ENTITY[]
                    {
                        ENTITY.ACTOR5
                    }
                },
                {
                    ENTITY.ACTOR5, new ENTITY[]
                    {
                        ENTITY.ACTOR3
                    }
                }
            };
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

        

        public IVisible FindEnemy(List<IVisible> vision)
        {
            return (from visible in vision
                    let imperil = (   from actor in _Actors.Values
                                    where actor.Imperil > 0 && actor.Id == visible.Id
                                    select actor.Imperil).Sum()
                    where imperil > 0 || _IsEnemy(visible.EntityType)
                    orderby imperil descending 
                    select visible).FirstOrDefault();

        }

        private bool _IsEnemy(ENTITY target)
        {
            ENTITY[] enemys;
            if (_Enemys.TryGetValue(_EntityType, out enemys))
            {
                return enemys.Any((e) => e == target);
            }
            return false;
        }

        public void Hate(Guid target, float damage)
        {

            Actor actor;
            if (_Actors.TryGetValue(target , out actor))
            {
                actor.Hate(damage);
            }
        }
    }

    
}