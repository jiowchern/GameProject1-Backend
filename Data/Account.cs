using System;
using System.Linq;


using ProtoBuf;


using Regulus.CustomType;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Data
{
	[ProtoContract]
	public class Account
	{
        public enum COMPETENCE
        {
            [EnumDescription("帳號管理")]
            ACCOUNT_MANAGER,

            [EnumDescription("遊戲體驗")]
            GAME_PLAYER,

            [EnumDescription("帳號查詢")]
            ACCOUNT_FINDER
        };

		[ProtoMember(1)]
		public Guid Id { get; set; }

		[ProtoMember(2)]
		public string Name { get; set; }

		[ProtoMember(3)]
		public string Password { get; set; }

        [ProtoMember(4)]
        public Flag<COMPETENCE> Competnces { get; set; }

		public Account()
		{
		
			Id = Guid.NewGuid();
			Name = Id.ToString();
			Password = Id.ToString(); 
		}

		public bool IsPassword(string password)
		{
			return Password == password;
		}


        public bool IsPlayer()
        {
            return _HasCompetence(COMPETENCE.GAME_PLAYER);
        }
        private bool _HasCompetence(COMPETENCE competence)
        {
            return Competnces[competence];
        }
        public bool HasCompetnce(COMPETENCE cOMPETENCE)
        {
            return _HasCompetence(cOMPETENCE);
        }
        public static Flag<COMPETENCE> AllCompetnce()
        {
            var flags = EnumHelper.GetEnums<COMPETENCE>().ToArray();            
            return new Flag<COMPETENCE>(flags);
        }
	}
}
