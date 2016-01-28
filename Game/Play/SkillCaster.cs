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

        public Vector2 GetPosition(float current)
        {
            var index = current / Data.Total;
            var idx = (int)Math.Ceiling(index);
            var position = Data.Roots[idx];
            return new Vector2();
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

        public bool CanDisarm()
        {
            return (from e in Data.Effects where e.Type == EFFECT_TYPE.DISARM select e.Value).Any();
        }

        public float GetShiftDirection()
        {
            return (from e in Data.Effects where e.Type == EFFECT_TYPE.SHIFT_DIRECTION select e.Value).Sum();
        }

        public float GetShiftSpeed()
        {
            return (from e in Data.Effects where e.Type == EFFECT_TYPE.SHIFT_SPEED select e.Value).Sum();
        }

        public float GetBackward()
        {
            return (from e in Data.Effects where e.Type == EFFECT_TYPE.MOVE_BACKWARD select e.Value).Sum();
        }

        public float GetForward()
        {
            return (from e in Data.Effects where e.Type == EFFECT_TYPE.MOVE_FORWARD select e.Value).Sum();
        }

        public float GetRunForward()
        {
            return (from e in Data.Effects where e.Type == EFFECT_TYPE.MOVE_RUNFORWARD select e.Value).Sum();
        }
        public float GetTurnLeft()
        {
            return (from e in Data.Effects where e.Type == EFFECT_TYPE.MOVE_TURNLEFT select e.Value).Sum();
        }

        public float GetTurnRight()
        {
            return (from e in Data.Effects where e.Type == EFFECT_TYPE.MOVE_TURNRIGHT select e.Value).Sum();
        }

        public bool IsBlock()
        {
            return (from e in Data.Effects where e.Type == EFFECT_TYPE.BLOCK select e.Value).Any();
        }

        public bool IsControll()
        {
            return (from e in Data.Effects
                    where 
                    e.Type == EFFECT_TYPE.MOVE_TURNRIGHT ||
                    e.Type == EFFECT_TYPE.MOVE_BACKWARD ||
                    e.Type == EFFECT_TYPE.MOVE_RUNFORWARD ||
                    e.Type == EFFECT_TYPE.MOVE_TURNLEFT ||
                    e.Type == EFFECT_TYPE.MOVE_FORWARD 
                    select e.Value).Sum() > 0.0f;
        }

        public bool IsSmash()
        {
            return (from e in Data.Effects where e.Type == EFFECT_TYPE.SMASH select e.Value).Any();
        }

        public bool IsPunch()
        {
            return (from e in Data.Effects where e.Type == EFFECT_TYPE.PUNCH select e.Value).Any();
        }
    }
}