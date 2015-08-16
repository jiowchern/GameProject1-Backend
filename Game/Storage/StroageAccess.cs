using System;
using System.Linq;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Storage
{
    public class StroageAccess : IStage, IQuitable, IStorageCompetences
    {
        public delegate void DoneCallback();

        public event DoneCallback DoneEvent;

        private readonly Account _Account;

        private readonly ISoulBinder _Binder;

        private readonly IStorage _Storage;

        public StroageAccess(ISoulBinder binder, Account account, IStorage storage)
        {
            _Binder = binder;
            _Account = account;
            _Storage = storage;
        }

        void IQuitable.Quit()
        {
            DoneEvent();
        }

        void IStage.Enter()
        {
            _Attach(_Account);
        }

        void IStage.Leave()
        {
            _Detach(_Account);
        }

        void IStage.Update()
        {
        }

        Value<Account.COMPETENCE[]> IStorageCompetences.Query()
        {
            return _Account.Competnces.ToArray();
        }

        Value<Guid> IStorageCompetences.QueryForId()
        {
            return _Account.Id;
        }

        private void _Attach(Account account)
        {
            _Binder.Bind<IStorageCompetences>(this);

            if (account.HasCompetnce(Account.COMPETENCE.ACCOUNT_FINDER))
            {
                _Binder.Bind<IAccountFinder>(_Storage);
                _Binder.Bind<IGameRecorder>(_Storage);
            }

            if (account.HasCompetnce(Account.COMPETENCE.ACCOUNT_MANAGER))
            {
                _Binder.Bind<IAccountManager>(_Storage);
            }

        }

        private void _Detach(Account account)
        {
            if (account.HasCompetnce(Account.COMPETENCE.ACCOUNT_FINDER))
            {
                _Binder.Unbind<IAccountFinder>(_Storage);
                _Binder.Unbind<IGameRecorder>(_Storage);
            }

            if (account.HasCompetnce(Account.COMPETENCE.ACCOUNT_MANAGER))
            {
                _Binder.Unbind<IAccountManager>(_Storage);
            }

            _Binder.Unbind<IStorageCompetences>(this);
        }
    }
}