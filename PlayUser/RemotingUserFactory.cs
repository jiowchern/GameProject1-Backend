using Regulus.Framework;
using Regulus.Remoting.Ghost.Native;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1
{
	public class RemotingUserFactory : IUserFactoty<IUser>
	{
		IUser IUserFactoty<IUser>.SpawnUser()
		{
		    var gpiProvider = new Regulus.Project.ItIsNotAGame1.GPIProvider();
            return new User(Agent.Create(gpiProvider));
		}

		ICommandParsable<IUser> IUserFactoty<IUser>.SpawnParser(Command command, Console.IViewer view, IUser user)
		{
			return new CommandParser(command, view, user);
		}
	}
}
