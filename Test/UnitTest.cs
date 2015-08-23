
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NSubstitute;

using Regulus.CustomType;
using Regulus.Project.ItIsNotAGame1.Game.Play;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Test
{
    [TestClass]
    public class UnitTest
    {

        [TestMethod]
        public void TestZone()
        {                         
            var rms = new RealmMaterial();
            rms.Name = "test";
            rms.EntityDatas = new EntityData[]
            {
                new EntityData {  }
            };
            var zone = new Zone(new RealmMaterial[] { rms });
            var map = zone.FindMap("test");
            Assert.AreNotEqual(null, map);
        }

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

            Polygon meshs1 = new Polygon(new[]{
                new Vector2(1, 1),
                new Vector2(2, 1),
                new Vector2(2, 2),
                new Vector2(1, 2)
            });
            Polygon meshs2 = new Polygon(new[]{
                new Vector2(1, 1),
                new Vector2(2, 1),
                new Vector2(2, 2),
                new Vector2(1, 2)
            });
            Polygon meshs3 = new Polygon(new[]{
                new Vector2(1, 1),
                new Vector2(2, 1),
                new Vector2(2, 2),
                new Vector2(1, 2)
            });
            Polygon meshs4 = new Polygon(new[]{
                new Vector2(1, 1),
                new Vector2(2, 1),
                new Vector2(2, 2),
                new Vector2(1, 2)
            });
            Polygon meshs5 = new Polygon(new[]{
                new Vector2(1, 1),
                new Vector2(2, 1),
                new Vector2(2, 2),
                new Vector2(1, 2)
            });
           
            meshs2.Offset(0,2);
            
            meshs3.Offset(0, 4);
            
            meshs4.Offset(0, 6);
            
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
