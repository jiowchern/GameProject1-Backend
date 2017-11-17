using Regulus.Framework;
using Regulus.Network;
using Regulus.Remoting;
using Regulus.Utility;


namespace Regulus.Project.ItIsNotAGame1
{
	public class RemotingUserFactory : IUserFactoty<IUser>
	{
	    private readonly IClient _Client;
        private readonly IProtocol _Protocol;

	    public RemotingUserFactory(IProtocol protocol, Regulus.Network.IClient client)
	    {
	        _Client = client;
            _Protocol = protocol;
	    }

	    IUser IUserFactoty<IUser>.SpawnUser()
		{
			
			return new User(Regulus.Remoting.Ghost.Native.Agent.Create(_Protocol, _Client));
		}

		ICommandParsable<IUser> IUserFactoty<IUser>.SpawnParser(Command command, Console.IViewer view, IUser user)
		{
			return new CommandParser(command, view, user);
		}
	}
}
