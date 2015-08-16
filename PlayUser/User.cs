using Regulus.Framework;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;




namespace Regulus.Project.ItIsNotAGame1
{
	internal class User : IUser
	{
		private readonly IAgent _Agent;

		private readonly Updater _Updater;

		private readonly Regulus.Remoting.User _User;

		public User(IAgent agent)
		{
			_Agent = agent;
			_Updater = new Updater();
			_User = new Regulus.Remoting.User(_Agent);
		}

		bool IUpdatable.Update()
		{
			_Updater.Working();
			return true;
		}

		void IBootable.Launch()
		{
			_Updater.Add(_User);
		}

		void IBootable.Shutdown()
		{
			_Updater.Shutdown();
		}

		Regulus.Remoting.User IUser.Remoting
		{
			get { return _User; }
		}

		INotifier<Regulus.Project.ItIsNotAGame1.Data.IVerify> IUser.VerifyProvider
		{
			get { return _Agent.QueryNotifier<Regulus.Project.ItIsNotAGame1.Data.IVerify>(); }
		}

		

		INotifier<Regulus.Project.ItIsNotAGame1.Data.IAccountStatus> IUser.AccountStatusProvider
		{
			get { return _Agent.QueryNotifier<IAccountStatus>(); }
		}

	
	}
}
