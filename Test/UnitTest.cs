using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
