using Regulus.Framework;
using Regulus.Network;
using Regulus.Remoting;
using Regulus.Utility;
using Agent = Regulus.Remoting.Ghost.Native.Agent;

namespace Regulus.Project.ItIsNotAGame1.Storage.User
{
	public class RemotingFactory : IUserFactoty<IUser>


	{
	    private readonly IProtocol _Protocol;
	    private readonly IClient _Client;

	    public RemotingFactory(IProtocol protocol , Regulus.Network.IClient client)
	    {
	        _Protocol = protocol;
	        _Client = client;
	    }

	    IUser IUserFactoty<IUser>.SpawnUser()
		{
			return new User(Agent.Create(_Protocol, _Client));
		}

		ICommandParsable<IUser> IUserFactoty<IUser>.SpawnParser(Command command, Console.IViewer view, IUser user)
		{
			return new CommandParser(command, view, user);
		}
	}
}
