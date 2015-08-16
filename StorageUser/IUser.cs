
using Regulus.Remoting;
using Regulus.Utility;



namespace Regulus.Project.ItIsNotAGame1.Storage.User
{
	public interface IUser : IUpdatable
	{
		Regulus.Remoting.User Remoting { get; }

		INotifier<Regulus.Project.ItIsNotAGame1.Data.IVerify> VerifyProvider { get; }

		INotifier<Regulus.Project.ItIsNotAGame1.Data.IStorageCompetences> StorageCompetencesProvider { get; }

		

		INotifier<T> QueryProvider<T>();
	}
}
