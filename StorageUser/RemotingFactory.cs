using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Remoting.Ghost.Native;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Storage.User
{
	public class RemotingFactory : IUserFactoty<IUser>


	{
	    private readonly IGhostProvider _Provider;

	    public RemotingFactory(IGhostProvider provider)
	    {
	        _Provider = provider;
	    }

	    IUser IUserFactoty<IUser>.SpawnUser()
		{
			return new User(Agent.Create(_Provider));
		}

		ICommandParsable<IUser> IUserFactoty<IUser>.SpawnParser(Command command, Console.IViewer view, IUser user)
		{
			return new CommandParser(command, view, user);
		}
	}
}
