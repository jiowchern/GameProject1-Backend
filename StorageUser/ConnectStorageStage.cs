using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Storage.User
{
	public class ConnectStorageStage : IStage
	{
		public delegate void DoneCallback(bool result);

		public event DoneCallback OnDoneEvent;

		private readonly string _IpAddress;

		private readonly int _Port;

		private readonly IUser _User;

		public ConnectStorageStage(IUser user, string ipaddress, int port)
		{
			// TODO: Complete member initialization
		    this._User = user;
		    this._IpAddress = ipaddress;
		    this._Port = port;
		}

		void IStage.Enter()
		{
		    this._User.Remoting.ConnectProvider.Supply += this._Connect;
		}

		void IStage.Leave()
		{
		    this._User.Remoting.ConnectProvider.Supply -= this._Connect;
		}

		void IStage.Update()
		{
		}

		private void _Connect(IConnect obj)
		{
			var result = obj.Connect(this._IpAddress, this._Port);
			result.OnValue += val => { this.OnDoneEvent(val); };
		}
	}
}
