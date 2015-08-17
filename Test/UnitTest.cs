using System;
using System.Globalization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using Regulus.CustomType;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Project.ItIsNotAGame1.Game.Play;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Test
{
    [TestClass]
    public class UnitTest
    {
        

        [TestMethod]
        public void TestCenter()
        {
            var binder = NSubstitute.Substitute.For<Regulus.Remoting.ISoulBinder>();
            var finder = NSubstitute.Substitute.For<Regulus.Project.ItIsNotAGame1.Data.IAccountFinder>();
            var record = NSubstitute.Substitute.For<Regulus.Project.ItIsNotAGame1.Data.IGameRecorder>();


            var center = new Center(finder, record);

            Regulus.Utility.Updater updater = new Updater();
            center.Join(binder);
            updater.Add(center);
            updater.Working();
            updater.Shutdown();
        }

        [TestMethod]
        public void TestMapFind()
        {
            Map map = new Map();

            Rect rect = new Rect(0.5f ,5,1,10);

            
            IObservable observable = NSubstitute.Substitute.For<IObservable>();
            observable.Vision.Returns(rect);

            IVisible[] visables = new Wall[]
            {
                new Wall(new Rect(1,1,1,1)), 
                new Wall(new Rect(1,3,1,1)), 
                new Wall(new Rect(1,5,1,1)), 
                new Wall(new Rect(1,7,1,1)), 
                new Wall(new Rect(1,9,1,1))
            };

            foreach (var visable in visables)
            {
                map.Join(visable);    
            }
            
            
            var results = map.Find(observable);


            foreach (var visable in visables)
            {
                map.Left(visable);
            }


            Assert.AreEqual(3, results.Length);                        
        }
         [TestMethod]
        public void TestMove()
        {
            Map map = new Map();

            Rect rect = new Rect(-0.5f, -0.5f , 1, 1);


            Mover mover = new Mover(rect , 1.0f);
            

            IVisible[] visables = new Wall[]
            {
                new Wall(new Rect(1,1,1,1)), 
                new Wall(new Rect(1,3,1,1)), 
                new Wall(new Rect(1,5,1,1)), 
                new Wall(new Rect(1,7,1,1)), 
                new Wall(new Rect(1,9,1,1))
            };

            foreach (var visable in visables)
            {
                map.Join(visable);
            }

            
            var results = map.Find(mover.GetOrbit(1.0f));


            foreach (var visable in visables)
            {
                map.Left(visable);
            }


            Assert.AreEqual(3, results.Length);        
        }
    }
}
