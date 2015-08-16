using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Framework;
using Regulus.Game;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Storage
{
    internal class User : Regulus.Game.IUser
    {
        private event OnQuit _QuitEvent;

        private event OnNewUser _VerifySuccessEvent;

        private readonly ISoulBinder _Binder;

        private readonly StageMachine _Machine;

        private readonly IStorage _Storage;

        private Account _Account;

        public User(ISoulBinder binder, IStorage storage)
        {
            _Storage = storage;
            _Binder = binder;
            _Machine = new StageMachine();
        }

        void Regulus.Game.IUser.OnKick(Guid id)
        {
        }

        event OnNewUser Regulus.Game.IUser.VerifySuccessEvent
        {
            add { _VerifySuccessEvent += value; }
            remove { _VerifySuccessEvent -= value; }
        }

        event OnQuit Regulus.Game.IUser.QuitEvent
        {
            add { _QuitEvent += value; }
            remove { _QuitEvent -= value; }
        }

        bool IUpdatable.Update()
        {
            _Machine.Update();
            return true;
        }

        void IBootable.Launch()
        {
            _ToVerify();
        }

        void IBootable.Shutdown()
        {
            _Machine.Termination();
        }

        private void _ToVerify()
        {
            var verify = _CreateVerify();
            _AddVerifyToStage(verify);
        }

        private Verify _CreateVerify()
        {
            _Account = null;
            var verify = new Verify(_Storage);
            return verify;
        }

        private void _AddVerifyToStage(Verify verify)
        {
            var stage = new VerifyStage(_Binder, verify);
            stage.DoneEvent += _VerifySuccess;
            _Machine.Push(stage);
        }

        private void _VerifySuccess(Account account)
        {
            _VerifySuccessEvent(account.Id);
            _Account = account;
            _ToRelease(account);
        }

        private void _ToRelease(Account account)
        {
            var stage = new StroageAccess(_Binder, account, _Storage);
            stage.DoneEvent += _ToVerify;
            _Machine.Push(stage);
        }
    }
}
