﻿// --------------------------------------------------------------------------------------------------------------------
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

		private readonly Regulus.Remoting.User _Remoting;

		private readonly Updater _Updater;

		public User(IAgent agent)
		{
			_Agent = agent;
			_Updater = new Updater();
			_Remoting = new Regulus.Remoting.User(agent);
		}

		Regulus.Remoting.User IUser.Remoting
		{
			get { return _Remoting; }
		}

		INotifier<Regulus.Project.ItIsNotAGame1.Data.IVerify> IUser.VerifyProvider
		{
			get { return _Agent.QueryNotifier<Regulus.Project.ItIsNotAGame1.Data.IVerify>(); }
		}

		bool IUpdatable.Update()
		{
			_Updater.Working();
			return true;
		}

		void IBootable.Launch()
		{
			_Updater.Add(_Agent);
			_Updater.Add(_Remoting);
		}

		void IBootable.Shutdown()
		{
			_Updater.Shutdown();
		}

		INotifier<T> IUser.QueryProvider<T>()
		{
			return _Agent.QueryNotifier<T>();
		}

		INotifier<Regulus.Project.ItIsNotAGame1.Data.IStorageCompetences> IUser.StorageCompetencesProvider
		{
			get { return _Agent.QueryNotifier<IStorageCompetences>(); }
		}
	}
}
