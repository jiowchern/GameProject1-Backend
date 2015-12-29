using System.Linq;

using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class ActorSkill
    {
        public readonly ACTOR_STATUS_TYPE[] Skills;

        private SkillData[] _Datas;

        public ActorSkill(SkillData[] datas)
        {
            _Datas = datas;
            Skills = (from d in _Datas select d.Id).ToArray();
        }

        public SkillCaster FindCaster(ACTOR_STATUS_TYPE skill)
        {
            var data = (from d in _Datas where d.Id == skill select d).SingleOrDefault();
            if(data != null)
                return new SkillCaster(data , new Determination(data.Lefts , data.Rights , data.Total , data.Begin , data.End));
            return null;
        }
    }
}