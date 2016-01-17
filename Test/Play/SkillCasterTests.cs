using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Project.ItIsNotAGame1.Game.Play;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Regulus.CustomType;
using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Game.Play.Tests
{
    [TestClass()]
    public class SkillCasterTests
    {
        [TestMethod()]
        public void GetPositionTest()
        {
            var data = new SkillData();
            data.Total = 10.0f;            
            /*data.Roots = new[]
            {
                new Vector2(0,0),
                new Vector2(1,0),                
            };*/
            var skill = new SkillCaster(data ,  null);
            var pos1 = skill.GetPosition(0);

            var pos2 = skill.GetPosition(1);

            Assert.AreEqual(0, pos1.X);
            Assert.AreEqual(0, pos1.Y);

            Assert.AreEqual(0.1f, pos2.X);
            Assert.AreEqual(0.1f, pos2.Y);

        }
    }
}