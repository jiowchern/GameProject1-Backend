using Regulus.Remoting;
using Regulus.Utility;



namespace Regulus.Project.ItIsNotAGame1
{
	public interface IUser : IUpdatable
	{
		Regulus.Remoting.User Remoting { get; }

		INotifier<Regulus.Project.ItIsNotAGame1.Data.IVerify> VerifyProvider { get; }

        INotifier<Regulus.Project.ItIsNotAGame1.Data.IVisible> VisibleProvider { get; }

        INotifier<Regulus.Project.ItIsNotAGame1.Data.IController> ControllerProvider { get; }

        INotifier<Regulus.Project.ItIsNotAGame1.Data.IAccountStatus> AccountStatusProvider { get; }
	}
}
