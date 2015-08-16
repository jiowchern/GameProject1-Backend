using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using Regulus.CustomType;

using Regulus.Remoting;
using Regulus.Utility;


namespace Regulus.Project.ItIsNotAGame1.Storage
{
    public class Server : Regulus.Remoting.ICore, Regulus.Project.ItIsNotAGame1.Data.IStorage
    {
        private readonly Regulus.Project.ItIsNotAGame1.Game.Storage.Center _Center;

        private readonly Regulus.NoSQL.Database _Database;

        private readonly string _DefaultAdministratorName;

        private readonly string _Ip;

        private readonly Regulus.Utility.LogFileRecorder _LogRecorder;

        private readonly string _Name;

        private readonly Regulus.Utility.Updater _Updater;

        

        public Server()
        {
            _LogRecorder = new Regulus.Utility.LogFileRecorder("Storage");
            _DefaultAdministratorName = "itisnotagame";
            _Ip = "mongodb://127.0.0.1:27017";
            _Name = "ItIsNotAGame1";
            _Updater = new Regulus.Utility.Updater();
            _Database = new Regulus.NoSQL.Database(_Ip);
            _Center = new Regulus.Project.ItIsNotAGame1.Game.Storage.Center(this);
        }

        void Regulus.Remoting.ICore.AssignBinder(Regulus.Remoting.ISoulBinder binder)
        {
            _Core.AssignBinder(binder);
        }

        public Regulus.Remoting.ICore _Core { get { return _Center; } }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Working();
            return true;
        }

        void Regulus.Framework.IBootable.Launch()
        {
            Regulus.Utility.Singleton<Regulus.Utility.Log>.Instance.RecordEvent += _LogRecorder.Record;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            _Updater.Add(_Center);
            _Database.Launch(_Name);

            _HandleAdministrator();

            _HandleGuest();

            
        }

        void Regulus.Framework.IBootable.Shutdown()
        {
            _Database.Shutdown();
            _Updater.Shutdown();

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            Regulus.Utility.Singleton<Regulus.Utility.Log>.Instance.RecordEvent -= _LogRecorder.Record;
        }

        Regulus.Remoting.Value<Regulus.Project.ItIsNotAGame1.Data.Account> Regulus.Project.ItIsNotAGame1.Data.IAccountFinder.FindAccountByName(string name)
        {
            var account = _Find(name);

            if (account != null)
            {
                return account;
            }

            return new Regulus.Remoting.Value<Regulus.Project.ItIsNotAGame1.Data.Account>(null);
        }

        Regulus.Remoting.Value<Regulus.Project.ItIsNotAGame1.Data.ACCOUNT_REQUEST_RESULT> Regulus.Project.ItIsNotAGame1.Data.IAccountCreator.Create(Regulus.Project.ItIsNotAGame1.Data.Account account)
        {
            var result = _Find(account.Name);
            if (result != null)
            {
                return Regulus.Project.ItIsNotAGame1.Data.ACCOUNT_REQUEST_RESULT.REPEAT;
            }

            _Database.Add(account);
            return Regulus.Project.ItIsNotAGame1.Data.ACCOUNT_REQUEST_RESULT.OK;
        }

        Regulus.Remoting.Value<Regulus.Project.ItIsNotAGame1.Data.ACCOUNT_REQUEST_RESULT> Regulus.Project.ItIsNotAGame1.Data.IAccountManager.Delete(string account)
        {
            var result = _Find(account);
            if (result != null && _Database.Remove<Regulus.Project.ItIsNotAGame1.Data.Account>(a => a.Id == result.Id))
            {
                return Regulus.Project.ItIsNotAGame1.Data.ACCOUNT_REQUEST_RESULT.OK;
            }

            return Regulus.Project.ItIsNotAGame1.Data.ACCOUNT_REQUEST_RESULT.NOTFOUND;
        }

        Value<Regulus.Project.ItIsNotAGame1.Data.Account[]> Regulus.Project.ItIsNotAGame1.Data.IAccountManager.QueryAllAccount()
        {
            return _QueryAllAccount();
        }

        Value<Regulus.Project.ItIsNotAGame1.Data.ACCOUNT_REQUEST_RESULT> Regulus.Project.ItIsNotAGame1.Data.IAccountManager.Update(Regulus.Project.ItIsNotAGame1.Data.Account account)
        {
            if (_Database.Update(account, a => a.Id == account.Id))
            {
                return Regulus.Project.ItIsNotAGame1.Data.ACCOUNT_REQUEST_RESULT.OK;
            }

            return Regulus.Project.ItIsNotAGame1.Data.ACCOUNT_REQUEST_RESULT.NOTFOUND;
        }

        Value<Regulus.Project.ItIsNotAGame1.Data.Account> Regulus.Project.ItIsNotAGame1.Data.IAccountFinder.FindAccountById(Guid accountId)
        {
            return _Find(accountId);
        }

        Value<Regulus.Project.ItIsNotAGame1.Data.GamePlayerRecord> Regulus.Project.ItIsNotAGame1.Data.IGameRecorder.Load(Guid account_id)
        {
            var val = new Value<Regulus.Project.ItIsNotAGame1.Data.GamePlayerRecord>();
            var account = _Find(account_id);
            if (account.IsPlayer())
            {
                var recordTask = _Database.Find<Regulus.Project.ItIsNotAGame1.Data.GamePlayerRecord>(r => r.Owner == account_id);
                recordTask.ContinueWith(
                    task =>
                    {
                        if (task.Result.Count > 0)
                        {
                            val.SetValue(task.Result.FirstOrDefault());
                        }
                        else
                        {
                            var newRecord = new Regulus.Project.ItIsNotAGame1.Data.GamePlayerRecord
                            {
                                Id = Guid.NewGuid(),
                                Owner = account_id,
                                
                            };
                            _Database.Add(newRecord).Wait();
                            val.SetValue(newRecord);
                        }
                    });
            }
            else
            {
                val.SetValue(null);
            }

            return val;
        }

        void Regulus.Project.ItIsNotAGame1.Data.IGameRecorder.Save(Regulus.Project.ItIsNotAGame1.Data.GamePlayerRecord record)
        {
            _Database.Update(record, r => r.Id == record.Id);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            _LogRecorder.Record(ex.ToString());
            _LogRecorder.Save();
        }

        private async void _HandleAdministrator()
        {
            var accounts = await _Database.Find<Regulus.Project.ItIsNotAGame1.Data.Account>(a => a.Name == _DefaultAdministratorName);

            if (accounts.Count == 0)
            {
                var account = new Regulus.Project.ItIsNotAGame1.Data.Account
                {
                    Id = Guid.NewGuid(),
                    Name = _DefaultAdministratorName,
                    Password = "20150815",                
                };

                await _Database.Add(account);

                
            }
        }

        private async void _HandleGuest()
        {
            var accounts = await _Database.Find<Regulus.Project.ItIsNotAGame1.Data.Account>(a => a.Name == "Guest");

            if (accounts.Count == 0)
            {
                var account = new Regulus.Project.ItIsNotAGame1.Data.Account
                {
                    Id = Guid.NewGuid(),
                    Name = "Guest",
                    Password = "guest",
                    
                };
                await _Database.Add(account);
            }
        }

        private Regulus.Project.ItIsNotAGame1.Data.Account _Find(string name)
        {
            var task = _Database.Find<Regulus.Project.ItIsNotAGame1.Data.Account>(a => a.Name == name);
            task.Wait();
            return task.Result.FirstOrDefault();
        }

        private Regulus.Project.ItIsNotAGame1.Data.Account _Find(Guid id)
        {
            var task = _Database.Find<Regulus.Project.ItIsNotAGame1.Data.Account>(a => a.Id == id);
            task.Wait();
            return task.Result.FirstOrDefault();
        }

        private Value<Regulus.Project.ItIsNotAGame1.Data.Account[]> _QueryAllAccount()
        {
            var val = new Value<Regulus.Project.ItIsNotAGame1.Data.Account[]>();
            var t = _Database.Find<Regulus.Project.ItIsNotAGame1.Data.Account>(a => true);
            t.ContinueWith(list => { val.SetValue(list.Result.ToArray()); });
            return val;
        }

        

        

        
    }
}
