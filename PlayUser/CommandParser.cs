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
		    this._Command = command;
		    this._View = view;
		    this._User = user;
		}

		void ICommandParsable<IUser>.Clear()
		{
		    this._DestroySystem();
		}

		void ICommandParsable<IUser>.Setup(IGPIBinderFactory factory)
		{
		    this._CreateSystem();

		    this._CreateConnect(factory);

		    this._CreateOnline(factory);

		    this._CreateVerify(factory);

		    this._CreateControll(factory);

            this._CreateDevelopActor(factory);

        }
	    

	    private void _DestroySystem()
		{

		}

		private void _ConnectResult(bool result)
		{
		    this._View.WriteLine(string.Format("Connect result {0}", result));
		}

		


		private void _CreateSystem()
		{
		}



        private void _CreateControll(IGPIBinderFactory factory)
        {
            
            var controller = factory.Create(this._User.MoveControllerProvider);
            //controller.Bind("Move[Angle]", gpi=> new CommandParamBuilder().Build<float>(gpi.Move));
            //controller.Bind("Stop", gpi => new CommandParamBuilder().Build(gpi.Stop));
        }

        private void _CreateVerify(IGPIBinderFactory factory)
		{
			var verify = factory.Create(this._User.VerifyProvider);
			verify.Bind(
				"Login[result,id,password]", 
				gpi => { return new CommandParamBuilder().BuildRemoting<string, string, bool>(gpi.Login, this._VerifyResult); });
		}

		private void _CreateOnline(IGPIBinderFactory factory)
		{
			var online = factory.Create(this._User.Remoting.OnlineProvider);
			online.Bind(
				"Ping", 
				gpi => { return new CommandParamBuilder().Build(() => { this._View.WriteLine("Ping : " + gpi.Ping); }); });
			online.Bind(gpi => gpi.Disconnect());
		}

		private void _CreateConnect(IGPIBinderFactory factory)
		{
			var connect = factory.Create(this._User.Remoting.ConnectProvider);
			connect.Bind(
				"Connect[result , ipaddr ,port]", 
				gpi => { return new CommandParamBuilder().BuildRemoting<string, int, bool>(gpi.Connect, this._ConnectResult); });
		}

        private void _CreateDevelopActor(IGPIBinderFactory factory)
        {
            var binder = factory.Create(this._User.DevelopActorProvider);
            binder.Bind<float>( (gpi , view ) => gpi.SetBaseView(view) );
            binder.Bind<float>((gpi, speed) => gpi.SetSpeed(speed));
            binder.Bind<string , int>((gpi, name , count) => gpi.CreateItem(name , count));
            binder.Bind<string, float>((gpi, name, quality) => gpi.MakeItem(name, quality));
            binder.Bind<float, float>((gpi, x, y) => gpi.SetPosition(x,y));
            binder.Bind<string>((gpi, realm) => gpi.SetRealm(realm));
        }

        private void _VerifyResult(bool result)
		{
		    this._View.WriteLine(string.Format("Verify result {0}", result));
		}
	}
}
