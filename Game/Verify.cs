using Regulus.Remoting;



namespace Regulus.Project.ItIsNotAGame1.Game
{
    public class Verify : Regulus.Project.ItIsNotAGame1.Data.IVerify
	{
        public delegate void DoneCallback(Regulus.Project.ItIsNotAGame1.Data.Account account);

		public event DoneCallback OnDoneEvent;

        private readonly Regulus.Project.ItIsNotAGame1.Data.IAccountFinder _Storage;

        public Verify(Regulus.Project.ItIsNotAGame1.Data.IAccountFinder storage)
		{
			_Storage = storage;
		}

        Value<bool> Regulus.Project.ItIsNotAGame1.Data.IVerify.Login(string id, string password)
		{
			var returnValue = new Value<bool>();
			var val = _Storage.FindAccountByName(id);
			val.OnValue += account =>
			{
				var found = account != null;
				if(found && account.IsPassword(password))
				{
					if(OnDoneEvent != null)
					{
						OnDoneEvent(account);
					}

					returnValue.SetValue(true);
				}
				else
				{
					returnValue.SetValue(false);
				}
			};
			return returnValue;
		}
	}
}
