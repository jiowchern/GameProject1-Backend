using Regulus.CustomType;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class SkillCaster
    {
        private Regulus.Utility.TimeCounter _Timer;
        public readonly SkillData Data;

        private readonly Determination _Determination;

        private float _Begin;
        public SkillCaster(SkillData data, Determination determination)
        {
            
            Data = data;
            _Determination = determination;
            

            _Timer = new TimeCounter();
            
        }
        

        public bool IsDone()
        {
            return _Timer.Second >= Data.Total;
        }

        public Polygon Find()
        {
            var end = _Timer.Second;            
            var result =  _Determination.Find(_Begin , end);
            _Begin = end;
            return result;
        }
    }
}