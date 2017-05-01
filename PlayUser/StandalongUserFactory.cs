// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandaloneUserFactory.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the StandaloneUserFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Regulus.Framework;

using Regulus.Remoting;
using Regulus.Remoting.Standalone;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1
{
	public class StandaloneUserFactory
		: IUserFactoty<IUser>
	{
		private readonly ICore _Standalone;

	    private readonly IProtocol _Protocol;

	    public StandaloneUserFactory(ICore core , IProtocol protocol)
	    {
	        this._Standalone = core;
	        _Protocol = protocol;
	    }

	    IUser IUserFactoty<IUser>.SpawnUser()
		{
			var agent = new Agent(_Protocol);
			agent.ConnectedEvent += () => { this._Standalone.AssignBinder(agent); };

			return new User(agent);
		}

		ICommandParsable<IUser> IUserFactoty<IUser>.SpawnParser(Command command, Console.IViewer view, IUser user)
		{
			return new CommandParser(command, view, user);
		}
	}
}
