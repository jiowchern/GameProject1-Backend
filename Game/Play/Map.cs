using System;
using System.Linq;
using System.Collections.Generic;

using Regulus.Collection;
using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Map 
    {

        
        private Regulus.Collection.QuadTree<Visible> _QuadTree;

        private List<Visible> _Set;

        public Map()
        {
            _Set = new List<Visible>();
            _QuadTree = new QuadTree<Visible>(new Size(2, 2), 100);
        }

        public class Visible : Regulus.Collection.IQuadObject
        {
            public Visible(IIndividual noumenon)
            {
                Noumenon = noumenon;
                
            }

            public void Initial()
            {
                Noumenon.BoundsEvent += _Changed; // this -> Noumenon.EventSet
            }

            public void Release()
            {
                Noumenon.BoundsEvent -= _Changed;
            }

            private void _Changed()
            {
                _BoundsChanged(this , EventArgs.Empty ); 
            }

            ~Visible()
            {
                
            }

            Rect IQuadObject.Bounds
            {
                get { return Noumenon.Bounds ; }
            }

            private event EventHandler _BoundsChanged;
            event EventHandler IQuadObject.BoundsChanged
            {
                add { _BoundsChanged += value; }
                remove { _BoundsChanged -= value; }
            }

            public IIndividual Noumenon { get; private set; }
        }

        public void Join(IIndividual individual)
        {
            var v = new Visible(individual);
            v.Initial();
            _Set.Add(v);
            _QuadTree.Insert(v);

            individual.SetPosition(Regulus.Utility.Random.Instance.NextFloat(0,10) , Regulus.Utility.Random.Instance.NextFloat(0, 10));
        }

        public void Left(IIndividual individual)
        {
            var results = _Set.FindAll(v => v.Noumenon.Id == individual.Id);
            foreach (var result in results)
            {
                _QuadTree.Remove(result);
                _Set.Remove(result);
                result.Release();
            }
        }

        public IIndividual[] Find(IObservable observable)
        {
            var results = _QuadTree.Query(observable.Vision);

            return (from r in results select r.Noumenon).ToArray();
        }
        
    }
}