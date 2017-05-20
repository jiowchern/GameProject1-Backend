using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Remoting.Ghost.Native;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1
{
	public class RemotingUserFactory : IUserFactoty<IUser>
	{
	    private readonly IProtocol _Protocol;

	    public RemotingUserFactory(IProtocol protocol)
	    {
	        _Protocol = protocol;
	    }

	    IUser IUserFactoty<IUser>.SpawnUser()
		{
			
			return new User(Agent.Create(_Protocol));
		}

		ICommandParsable<IUser> IUserFactoty<IUser>.SpawnParser(Command command, Console.IViewer view, IUser user)
		{
			return new CommandParser(command, view, user);
		}
	}
}
