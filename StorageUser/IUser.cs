
using Regulus.Remoting;
using Regulus.Utility;



namespace Regulus.Project.ItIsNotAGame1.Storage.User
{
	public interface IUser : IUpdatable
	{
		Remoting.User Remoting { get; }

		INotifier<Data.IVerify> VerifyProvider { get; }

		INotifier<Data.IStorageCompetences> StorageCompetencesProvider { get; }

		

		INotifier<T> QueryProvider<T>();
	}
}
