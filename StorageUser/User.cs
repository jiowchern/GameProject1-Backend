// --------------------------------------------------------------------------------------------------------------------
// <copyright file="User.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the User type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Regulus.Framework;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;



namespace Regulus.Project.ItIsNotAGame1.Storage.User
{
	internal class User : IUser
	{
		private readonly IAgent _Agent;

		private readonly Remoting.User _Remoting;

		private readonly Updater _Updater;

		public User(IAgent agent)
		{
		    this._Agent = agent;
		    this._Updater = new Updater();
		    this._Remoting = new Remoting.User(agent);
		}

		Remoting.User IUser.Remoting
		{
			get { return this._Remoting; }
		}

		INotifier<IVerify> IUser.VerifyProvider
		{
			get { return this._Agent.QueryNotifier<IVerify>(); }
		}

		bool IUpdatable.Update()
		{
		    this._Updater.Working();
			return true;
		}

		void IBootable.Launch()
		{
		    this._Updater.Add(this._Agent);
		    this._Updater.Add(this._Remoting);
		}

		void IBootable.Shutdown()
		{
		    this._Updater.Shutdown();
		}

		INotifier<T> IUser.QueryProvider<T>()
		{
			return this._Agent.QueryNotifier<T>();
		}

		INotifier<IStorageCompetences> IUser.StorageCompetencesProvider
		{
			get { return this._Agent.QueryNotifier<IStorageCompetences>(); }
		}
	}
}
