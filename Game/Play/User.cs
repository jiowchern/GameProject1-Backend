using System;
using Regulus.Framework;
using Regulus.Game;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class User : Regulus.Game.IUser, Regulus.Project.ItIsNotAGame1.Data.IAccountStatus
    {
        private event Action _KickEvent;

        private event OnQuit _QuitEvent;

        private event OnNewUser _VerifySuccessEvent;

        private readonly Regulus.Project.ItIsNotAGame1.Data.IAccountFinder _AccountFinder;

        private readonly ISoulBinder _Binder;

        

        private readonly StageMachine _Machine;

        private readonly Regulus.Project.ItIsNotAGame1.Data.IGameRecorder _GameRecorder;

        private readonly Zone _Zone;

        private Regulus.Project.ItIsNotAGame1.Data.Account _Account;

        private Regulus.Project.ItIsNotAGame1.Data.GamePlayerRecord _GamePlayerRecord;

        public User(ISoulBinder binder, IAccountFinder account_finder, IGameRecorder game_record_handler, Zone zone)
        {
            _Machine = new StageMachine();

            _Binder = binder;
            _AccountFinder = account_finder;
            _GameRecorder = game_record_handler;
            _Zone = zone;
        }

        event Action Regulus.Project.ItIsNotAGame1.Data.IAccountStatus.KickEvent
        {
            add { _KickEvent += value; }
            remove { _KickEvent -= value; }
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

        void Regulus.Game.IUser.OnKick(Guid id)
        {
            if (_Account != null && _Account.Id == id)
            {
                if (_KickEvent != null)
                {
                    _KickEvent();
                }

                _ToVerify();
            }
        }

        bool IUpdatable.Update()
        {
            _Machine.Update();
            return true;
        }

        void IBootable.Launch()
        {
            _Binder.BreakEvent += _Quit;
            _Binder.Bind<Regulus.Project.ItIsNotAGame1.Data.IAccountStatus>(this);
            _ToVerify();
        }

        void IBootable.Shutdown()
        {
            _SaveRecord();
            _Binder.Unbind<Regulus.Project.ItIsNotAGame1.Data.IAccountStatus>(this);
            _Machine.Termination();
            _Binder.BreakEvent -= _Quit;
        }

        private void _Quit()
        {
            _QuitEvent();
        }

        private void _SaveRecord()
        {
            if (_GamePlayerRecord != null)
            {
                _GameRecorder.Save(_GamePlayerRecord);
            }
        }

        private void _ToVerify()
        {
            var verify = _CreateVerify();
            _AddVerifyToStage(verify);
        }

        private Verify _CreateVerify()
        {
            _Account = null;
            var verify = new Verify(_AccountFinder);
            return verify;
        }

        private void _AddVerifyToStage(Verify verify)
        {
            var stage = new VerifyStage(_Binder, verify);
            stage.DoneEvent += _VerifySuccess;
            _Machine.Push(stage);
        }

        private void _VerifySuccess(Regulus.Project.ItIsNotAGame1.Data.Account account)
        {
            _VerifySuccessEvent(account.Id);
            _Account = account;
            _ToLoadRecord();
        }

        private void _ToLoadRecord()
        {
            var stage = new LoadRecordStage(_Account.Id , _Binder, _GameRecorder);
            stage.DoneEvent += _ToRequestMap; 
            _Machine.Push(stage);
        }

        private void _ToRequestMap(GamePlayerRecord record)
        {
            _GamePlayerRecord = record;
            Map map = _Zone.FindMap("test");
            _ToGame(record, map);
        }

        private void _ToGame(GamePlayerRecord record , Map map)
        {            
            var stage = new GameStage(_Binder, record , _GameRecorder , map);
            stage.DoneEvent += _ToVerify;
            _Machine.Push(stage);
        }
    }
}