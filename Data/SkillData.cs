using System;

using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    [Serializable]
    public class SkillData
    {
        public ACTOR_STATUS_TYPE Id;

        public Vector2[] Lefts;

        public Vector2[] Rights;

        public Vector2[] Roots;

        public float Total;


        public float Begin;

        public float End;

        public ACTOR_STATUS_TYPE[] Nexts;

        public bool Block;

        public bool Smash;

        public SkillData()
        {
            Lefts = new Vector2[0];
            Rights = new Vector2[0];
            Nexts = new ACTOR_STATUS_TYPE[0];
            Roots = new Vector2[0];
        }

        public bool Punch;
    }
}
