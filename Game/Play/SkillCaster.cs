using System;
using System.Diagnostics;
using System.Linq;

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

        private float _Interval;

        public SkillCaster(SkillData data, Determination determination)
        {
            _Interval = data.Total / data.Roots.Length;
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

        public Vector2 GetPosition(float current)
        {
            var index = current / Data.Total;
            var idx = (int)Math.Ceiling(index);
            var position = Data.Roots[idx];
            return new Vector2();
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

        public SkillCaster FindNext(ACTOR_STATUS_TYPE skill)
        {
            var skillPrototype = Resource.Instance.FindSkill(skill);
            if (skillPrototype != null && Data.Nexts.Any(s => s == skill))
            {
                return new SkillCaster(skillPrototype, new Determination(skillPrototype));
            }
            return null;
        }
    }
}