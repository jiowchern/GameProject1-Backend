using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using Regulus.CustomType;

using Regulus.Remoting;
using Regulus.Utility;


namespace Regulus.Project.ItIsNotAGame1.Storage
{
    public class Server : ICore, Data.IStorage
    {
        private readonly Game.Storage.Center _Center;

        private readonly NoSQL.Database _Database;

        private readonly string _DefaultAdministratorName;

        private readonly string _Ip;

        private readonly LogFileRecorder _LogRecorder;

        private readonly string _Name;

        private readonly Updater _Updater;

        

        public Server()
        {
            this._LogRecorder = new LogFileRecorder("Storage");
            this._DefaultAdministratorName = "itisnotagame";
            this._Ip = "mongodb://127.0.0.1:27017";
            this._Name = "ItIsNotAGame1";
            this._Updater = new Updater();
            this._Database = new NoSQL.Database(this._Ip);
            this._Center = new Game.Storage.Center(this);
        }

        void ICore.AssignBinder(ISoulBinder binder)
        {
            this._Core.AssignBinder(binder);
        }

        public ICore _Core { get { return this._Center; } }

        bool IUpdatable.Update()
        {
            this._Updater.Working();
            return true;
        }

        void Framework.IBootable.Launch()
        {
            Singleton<Log>.Instance.RecordEvent += this._LogRecorder.Record;
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;

            this._Updater.Add(this._Center);
            this._Database.Launch(this._Name);

            this._HandleAdministrator();

            

            
        }

        void Framework.IBootable.Shutdown()
        {
            this._Database.Shutdown();
            this._Updater.Shutdown();

            AppDomain.CurrentDomain.UnhandledException -= this.CurrentDomain_UnhandledException;
            Singleton<Log>.Instance.RecordEvent -= this._LogRecorder.Record;
        }

        Value<Data.Account> Data.IAccountFinder.FindAccountByName(string name)
        {
            var account = this._Find(name);

            if (account != null)
            {
                return account;
            }

            return new Value<Data.Account>(null);
        }

        

        Value<Data.ACCOUNT_REQUEST_RESULT> Data.IAccountManager.Delete(string account)
        {
            var result = this._Find(account);
            if (result != null && this._Database.Remove<Data.Account>(a => a.Id == result.Id))
            {
                return Data.ACCOUNT_REQUEST_RESULT.OK;
            }

            return Data.ACCOUNT_REQUEST_RESULT.NOTFOUND;
        }

        Value<Data.Account[]> Data.IAccountManager.QueryAllAccount()
        {
            return this._QueryAllAccount();
        }

        Value<Data.ACCOUNT_REQUEST_RESULT> Data.IAccountManager.Update(Data.Account account)
        {
            if (this._Database.Update(account, a => a.Id == account.Id))
            {
                return Data.ACCOUNT_REQUEST_RESULT.OK;
            }

            return Data.ACCOUNT_REQUEST_RESULT.NOTFOUND;
        }

        Value<Data.Account> Data.IAccountFinder.FindAccountById(Guid accountId)
        {
            return this._Find(accountId);
        }

        Value<Data.GamePlayerRecord> Data.IGameRecorder.Load(Guid account_id)
        {
            var val = new Value<Data.GamePlayerRecord>();
            var account = this._Find(account_id);
            if (account.IsPlayer())
            {
                var recordTask = this._Database.Find<Data.GamePlayerRecord>(r => r.Owner == account_id);
                recordTask.ContinueWith(
                    task =>
                    {
                        if (task.Result.Count > 0)
                        {
                            val.SetValue(task.Result.FirstOrDefault());
                        }
                        else
                        {
                            var newRecord = new Data.GamePlayerRecord
                            {
                                Id = Guid.NewGuid(),
                                Owner = account_id,
                                
                            };
                            this._Database.Add(newRecord).Wait();
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

        void Data.IGameRecorder.Save(Data.GamePlayerRecord record)
        {
            this._Database.Update(record, r => r.Id == record.Id);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            this._LogRecorder.Record(ex.ToString());
            this._LogRecorder.Save();
        }

        private async void _HandleAdministrator()
        {
            var accounts = await this._Database.Find<Data.Account>(a => a.Name == this._DefaultAdministratorName);

            if (accounts.Count == 0)
            {
                var account = new Data.Account
                {
                    Id = Guid.NewGuid(),
                    Name = this._DefaultAdministratorName,
                    Password = "20150815",
                    Competnces = Data.Account.AllCompetnce()
                };

                await this._Database.Add(account);

                
            }
        }

        

        private Data.Account _Find(string name)
        {
            var task = this._Database.Find<Data.Account>(a => a.Name == name);
            task.Wait();
            return task.Result.FirstOrDefault();
        }

        private Data.Account _Find(Guid id)
        {
            var task = this._Database.Find<Data.Account>(a => a.Id == id);
            task.Wait();
            return task.Result.FirstOrDefault();
        }

        private Value<Data.Account[]> _QueryAllAccount()
        {
            var val = new Value<Data.Account[]>();
            var t = this._Database.Find<Data.Account>(a => true);
            t.ContinueWith(list => { val.SetValue(list.Result.ToArray()); });
            return val;
        }







        Value<Data.ACCOUNT_REQUEST_RESULT> Data.IAccountManager.Create(Data.Account account)
        {
            var result = this._Find(account.Name);
            if (result != null)
            {
                return Data.ACCOUNT_REQUEST_RESULT.REPEAT;
            }

            this._Database.Add(account);
            return Data.ACCOUNT_REQUEST_RESULT.OK;
        }
    }
}
