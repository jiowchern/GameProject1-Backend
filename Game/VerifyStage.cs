using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ItIsNotAGame1.Game
{
	class VerifyStage : Utility.IStage
	{
		public event Verify.DoneCallback DoneEvent;

		private readonly Remoting.ISoulBinder _Binder;

		private readonly Verify _Verify;


	    
		public VerifyStage(Remoting.ISoulBinder binder, Verify verify)
		{
		    this._Verify = verify;
		    this._Binder = binder;
		}

		void Utility.IStage.Enter()
		{
		    this._Verify.OnDoneEvent += this.DoneEvent;

		    this._Binder.Bind<Regulus.Project.ItIsNotAGame1.Data.IVerify>(this._Verify);
		}

		void Utility.IStage.Leave()
		{
		    this._Binder.Unbind<Regulus.Project.ItIsNotAGame1.Data.IVerify>(this._Verify);
		    this._Verify.OnDoneEvent -= this.DoneEvent;
		}

		void Utility.IStage.Update()
		{
		}
	}
}
