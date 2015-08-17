using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ItIsNotAGame1.Game
{
	class VerifyStage : Regulus.Utility.IStage
	{
		public event Verify.DoneCallback DoneEvent;

		private readonly Regulus.Remoting.ISoulBinder _Binder;

		private readonly Verify _Verify;

		public VerifyStage(Regulus.Remoting.ISoulBinder binder, Verify verify)
		{
			_Verify = verify;
			_Binder = binder;
		}

		void Regulus.Utility.IStage.Enter()
		{
			_Verify.OnDoneEvent += DoneEvent;

			_Binder.Bind<Regulus.Project.ItIsNotAGame1.Data.IVerify>(_Verify);
		}

		void Regulus.Utility.IStage.Leave()
		{
			_Binder.Unbind<Regulus.Project.ItIsNotAGame1.Data.IVerify>(_Verify);
			_Verify.OnDoneEvent -= DoneEvent;
		}

		void Regulus.Utility.IStage.Update()
		{
		}
	}
}
