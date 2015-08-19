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
        public void TestMove()
        {
            //Mover mover = new Mover();
        }

        [TestMethod]
        public void TestMapFind()
        {
            Map map = new Map();

            Rect rect = new Rect(0.5f ,5,1,10);

            
            IObservable observable = NSubstitute.Substitute.For<IObservable>();
            observable.Vision.Returns(rect);

            Polygon meshs1 = new Polygon();
            Polygon meshs2 = new Polygon();
            Polygon meshs3 = new Polygon();
            Polygon meshs4 = new Polygon();
            Polygon meshs5 = new Polygon();
            
            meshs1.Points.Add(new Vector2(1, 1));
            meshs1.Points.Add(new Vector2(2, 1));
            meshs1.Points.Add(new Vector2(2, 2));
            meshs1.Points.Add(new Vector2(1, 2));

            meshs2.Points.Add(new Vector2(1, 1));
            meshs2.Points.Add(new Vector2(2, 1));
            meshs2.Points.Add(new Vector2(2, 2));
            meshs2.Points.Add(new Vector2(1, 2));
            meshs2.Offset(0,2);

            meshs3.Points.Add(new Vector2(1, 1));
            meshs3.Points.Add(new Vector2(2, 1));
            meshs3.Points.Add(new Vector2(2, 2));
            meshs3.Points.Add(new Vector2(1, 2));
            meshs3.Offset(0, 4);

            meshs4.Points.Add(new Vector2(1, 1));
            meshs4.Points.Add(new Vector2(2, 1));
            meshs4.Points.Add(new Vector2(2, 2));
            meshs4.Points.Add(new Vector2(1, 2));
            meshs4.Offset(0, 6);

            meshs5.Points.Add(new Vector2(1, 1));
            meshs5.Points.Add(new Vector2(2, 1));
            meshs5.Points.Add(new Vector2(2, 2));
            meshs5.Points.Add(new Vector2(1, 2));
            meshs5.Offset(0, 8);

            IVisible[] visables = {
                new Entity(meshs1), 
                new Entity(meshs2), 
                new Entity(meshs3), 
                new Entity(meshs4), 
                new Entity(meshs5)
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
        
    }
}
