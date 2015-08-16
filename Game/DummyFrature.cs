

using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using System;
using System.Collections.Generic;
namespace Regulus.Project.ItIsNotAGame1.Game
{
    public class DummyFrature :  IStorage
    {
        private readonly List<Account> _Accounts;

        private readonly List<GamePlayerRecord> _Records;

        public DummyFrature()
        {
            _Records = new List<GamePlayerRecord>();

            _Accounts = new List<Account>
			{
				new Account
				{
					Id = Guid.NewGuid(), 
					Password = "pw", 
					Name = "name", 
					Competnces = Account.AllCompetnce()
				}, 
				new Account
				{
					Id = Guid.NewGuid(), 
					Password = "20150815", 
					Name = "itisnotagame", 
					Competnces = Account.AllCompetnce()
				}, 
				new Account
				{
					Id = Guid.NewGuid(), 
					Password = "user", 
					Name = "user1", 
					Competnces = Account.AllCompetnce()
				}
			};
        }

        Value<Account> IAccountFinder.FindAccountByName(string id)
        {
            return _Accounts.Find(a => a.Name == id);
        }

        Value<Account> IAccountFinder.FindAccountById(Guid account_id)
        {
            return _Accounts.Find(a => a.Id == account_id);
        }

        

        Value<Account[]> IAccountManager.QueryAllAccount()
        {
            return _Accounts.ToArray();
        }

        

        Value<ACCOUNT_REQUEST_RESULT> IAccountManager.Delete(string account)
        {
            _Accounts.RemoveAll(a => a.Name == account);
            return ACCOUNT_REQUEST_RESULT.OK;
        }

        Value<ACCOUNT_REQUEST_RESULT> IAccountManager.Update(Account account)
        {
            if (_Accounts.RemoveAll(a => a.Id == account.Id) > 0)
            {
                _Accounts.Add(account);
                return ACCOUNT_REQUEST_RESULT.OK;
            }

            return ACCOUNT_REQUEST_RESULT.NOTFOUND;
        }

        Value<GamePlayerRecord> IGameRecorder.Load(Guid account_id)
        {
            var account = _Accounts.Find(a => a.Id == account_id);
            if (account.IsPlayer())
            {
                var record = _Records.Find(r => r.Owner == account.Id);
                if (record == null)
                {
                    record = new GamePlayerRecord
                    {
                        Id = Guid.NewGuid(),
        
                        Owner = account_id
                    };
                }

                return record;
            }

            return null;
        }

        void IGameRecorder.Save(GamePlayerRecord game_player_record)
        {
            var account = _Accounts.Find(a => a.Id == game_player_record.Owner);
            if (account.IsPlayer())
            {
                var old = _Records.Find(r => r.Owner == account.Id);
                _Records.Remove(old);
                _Records.Add(game_player_record);
            }
        }



        Value<ACCOUNT_REQUEST_RESULT> IAccountManager.Create(Account account)
        {
            _Accounts.Add(account);
            return ACCOUNT_REQUEST_RESULT.OK;
        }
    }
}