using System;
using System.Linq;
using System.Collections.Generic;

using Regulus.Collection;
using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Map 
    {

        
        private readonly QuadTree<Visible> _QuadTree;

        private readonly List<Visible> _Set;

        public Map()
        {
            this._Set = new List<Visible>();
            this._QuadTree = new QuadTree<Visible>(new Size(2, 2), 100);
        }

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
                this._BoundsChanged(this , EventArgs.Empty ); 
            }

            ~Visible()
            {
                
            }

            Rect IQuadObject.Bounds
            {
                get { return this.Noumenon.Bounds ; }
            }

            private event EventHandler _BoundsChanged;
            event EventHandler IQuadObject.BoundsChanged
            {
                add { this._BoundsChanged += value; }
                remove { this._BoundsChanged -= value; }
            }

            public IIndividual Noumenon { get; private set; }
        }

        public void JoinStaff(IIndividual individual)
        {
            this._Join(individual);
        }
        public void JoinChallenger(IIndividual individual)
        {
            this._Join(individual);
            individual.SetPosition(Utility.Random.Instance.NextFloat(0,2.5f) , Utility.Random.Instance.NextFloat(0, 2.5f));
        }

        private void _Join(IIndividual individual)
        {
            var v = new Visible(individual);
            v.Initial();
            this._Set.Add(v);
            this._QuadTree.Insert(v);
        }

        public void Left(IIndividual individual)
        {
            var results = this._Set.FindAll(v => v.Noumenon.Id == individual.Id);
            foreach (var result in results)
            {
                this._QuadTree.Remove(result);
                this._Set.Remove(result);
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