using Regulus.Remoting;



namespace Regulus.Project.ItIsNotAGame1.Game
{
	public class Verify : Data.IVerify
	{
		public delegate void DoneCallback(Data.Account account);

		public event DoneCallback OnDoneEvent;

		private readonly Data.IAccountFinder _Storage;

		public Verify(Data.IAccountFinder storage)
		{
			this._Storage = storage;
		}

		Value<bool> Data.IVerify.Login(string id, string password)
		{
			var returnValue = new Value<bool>();
			var val = this._Storage.FindAccountByName(id);
			val.OnValue += account =>
			{
				var found = account != null;
				if(found && account.IsPassword(password))
				{
					if(this.OnDoneEvent != null)
					{
						this.OnDoneEvent(account);
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
