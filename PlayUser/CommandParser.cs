using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;




namespace Regulus.Project.ItIsNotAGame1
{
	public class CommandParser : ICommandParsable<IUser>
	{
		private readonly Command _Command;

		private readonly IUser _User;

		private readonly Console.IViewer _View;

		public CommandParser(Command command, Console.IViewer view, IUser user)
		{
			_Command = command;
			_View = view;
			_User = user;
		}

		void ICommandParsable<IUser>.Clear()
		{
			_DestroySystem();
		}

		void ICommandParsable<IUser>.Setup(IGPIBinderFactory factory)
		{
			_CreateSystem();

			_CreateConnect(factory);

			_CreateOnline(factory);

			_CreateVerify(factory);

		}

		private void _DestroySystem()
		{

		}

		private void _ConnectResult(bool result)
		{
			_View.WriteLine(string.Format("Connect result {0}", result));
		}

		


		private void _CreateSystem()
		{
		}

	
		

		private void _CreateVerify(IGPIBinderFactory factory)
		{
			var verify = factory.Create(_User.VerifyProvider);
			verify.Bind(
				"Login[result,id,password]", 
				gpi => { return new CommandParamBuilder().BuildRemoting<string, string, bool>(gpi.Login, _VerifyResult); });
		}

		private void _CreateOnline(IGPIBinderFactory factory)
		{
			var online = factory.Create(_User.Remoting.OnlineProvider);
			online.Bind(
				"Ping", 
				gpi => { return new CommandParamBuilder().Build(() => { _View.WriteLine("Ping : " + gpi.Ping); }); });
			online.Bind(gpi => gpi.Disconnect());
		}

		private void _CreateConnect(IGPIBinderFactory factory)
		{
			var connect = factory.Create(_User.Remoting.ConnectProvider);
			connect.Bind(
				"Connect[result , ipaddr ,port]", 
				gpi => { return new CommandParamBuilder().BuildRemoting<string, int, bool>(gpi.Connect, _ConnectResult); });
		}

		private void _VerifyResult(bool result)
		{
			_View.WriteLine(string.Format("Verify result {0}", result));
		}
	}
}
