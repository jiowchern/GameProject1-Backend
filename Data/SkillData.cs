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

        public Translate[] Roots;

        public float Total;

        public float Begin;

        public float End;

        public ACTOR_STATUS_TYPE[] Nexts;

        public bool Block;

        public bool Smash;

        public bool Punch;

        public bool Controll;

        public bool Disarm;

        public float Direction;

        public float Speed;

        public SkillData()
        {
            Lefts = new Vector2[0];
            Rights = new Vector2[0];
            Nexts = new ACTOR_STATUS_TYPE[0];
            Roots = new Translate[0];
        }

        
    }
}
