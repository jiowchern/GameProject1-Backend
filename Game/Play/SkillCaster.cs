using System.Diagnostics;

using Regulus.CustomType;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class SkillCaster
    {
        private readonly Regulus.Utility.TimeCounter _Timer;
        public readonly SkillData Data;

        private readonly Determination _Determination;

        
        public SkillCaster(SkillData data, Determination determination)
        {
            
            Data = data;
            _Determination = determination;
            

            _Timer = new TimeCounter();
            
        }

        public int Damage { get; set; }

        public bool IsDone(float second)
        {
            return _Timer.Second >= Data.Total;
        }

        public Polygon FindDetermination(float begin , float end)
        {            
            var result =  _Determination.Find(begin, end - begin);            
            return result;
        }

        public bool IsBlock()
        {
            return Data.Block;
        }

        public bool IsSmash()
        {
            return Data.Smash;
        }

        public bool IsPunch()
        {
            return Data.Punch;
        }

        public static SkillCaster Build(ACTOR_STATUS_TYPE type)
        {
            var skill = Resource.Instance.FindSkill(type);
            return new SkillCaster(skill , new Determination(skill));
        }
    }
}