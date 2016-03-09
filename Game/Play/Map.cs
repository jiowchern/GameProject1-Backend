using System;
using System.Linq;
using System.Collections.Generic;

using Regulus.Collection;
using Regulus.CustomType;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Extension;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Map 
    {
        public class Visible : IQuadObject
        {
            public Visible(IIndividual noumenon)
            {
                this.Noumenon = noumenon;
            }

            public void Initial()
            {
                this.Noumenon.BoundsEvent += this._Changed; // this -> Noumenon.EventSet
            }

            public void Release()
            {
                this.Noumenon.BoundsEvent -= this._Changed;
            }

            private void _Changed()
            {
                this._BoundsChanged(this, EventArgs.Empty);
            }

            ~Visible()
            {

            }

            Rect IQuadObject.Bounds
            {
                get { return this.Noumenon.Bounds; }
            }

            private event EventHandler _BoundsChanged;
            event EventHandler IQuadObject.BoundsChanged
            {
                add { this._BoundsChanged += value; }
                remove { this._BoundsChanged -= value; }
            }

            public IIndividual Noumenon { get; private set; }
        }

        private readonly QuadTree<Visible> _QuadTree;

        private readonly List<Visible> _Set;

        private readonly List<Visible> _EntranceSet;

        private Regulus.Utility.IRandom _Random;
        public Map()
        {
            _Random = Regulus.Utility.Random.Instance;
            _EntranceSet = new List<Visible>();
            this._Set = new List<Visible>();
            this._QuadTree = new QuadTree<Visible>(new Size(2, 2), 100);
        }

        public Map(Regulus.Utility.IRandom random)
        {
            _Random = random;
            _EntranceSet = new List<Visible>();
            this._Set = new List<Visible>();
            this._QuadTree = new QuadTree<Visible>(new Size(2, 2), 100);
        }



        public void JoinStaff(IIndividual individual)
        {
            this._Join(individual);
        }
        public bool JoinChallenger(IIndividual individual)
        {            
            this._Join(individual);


            var concierges = this._FindConcierges(individual);
            
            var concierge = concierges.Shuffle().FirstOrDefault();
            if(concierge != null)
            {
                Vector2 position = concierge.GetPosition();
                individual.SetPosition(position.X, position.Y);
                individual.AddDirection(_Random.NextInt(0, 360));
                return true;
            }
            return false;
        }

        private IEnumerable<Concierge> _FindConcierges(IIndividual individual)
        {
            return (from e in this._EntranceSet
                    let concierge = e.Noumenon.GetConcierge()
                    where concierge != null && concierge.IsAcceptsType(individual)
                    select concierge);
        }

        private void _Join(IIndividual individual)
        {
            var v = new Visible(individual);
            v.Initial();
            this._Set.Add(v);
            this._QuadTree.Insert(v);

            if(individual.EntityType == ENTITY.ENTRANCE)
            {
                _EntranceSet.Add(v);
            }
            
        }

        public void Left(IIndividual individual)
        {
            var results = this._Set.FindAll(v => v.Noumenon.Id == individual.Id);
            foreach (var result in results)
            {
                this._QuadTree.Remove(result);
                this._Set.Remove(result);
                _EntranceSet.Remove(result);
                result.Release();
            }
        }

        public IIndividual[] Find(Rect bound)
        {
            var results = this._QuadTree.Query(bound);

            return (from r in results select r.Noumenon).ToArray();
        }
        
    }
}