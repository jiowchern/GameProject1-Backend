using Regulus.Project.ItIsNotAGame1.Game.Play;
using System;
using System.Reflection.Emit;

using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Play
{
    public class Server : Remoting.ICore
    {
        private readonly Utility.LogFileRecorder _LogRecorder;

        private readonly Utility.StageMachine _Machine;

        private readonly Utility.Updater _Updater;

        private Center _Center;

        private Storage.User.Proxy _Storage;

        private Storage.User.IUser _StorageUser;

        private CustomType.Verify _StorageVerifyData;

        public Server()
        {
            this._LogRecorder = new Utility.LogFileRecorder("Play");

            this._StorageVerifyData = new CustomType.Verify();

            this._Machine = new Utility.StageMachine();
            this._Updater = new Utility.Updater();

            this._BuildParams();
            this._BuildUser();
        }

        void Remoting.ICore.AssignBinder(Remoting.ISoulBinder binder)
        {
            this._Center.Join(binder);
        }

        bool Utility.IUpdatable.Update()
        {
            this._Updater.Working();
            this._Machine.Update();
            return true;
        }

        void Framework.IBootable.Shutdown()
        {
            this._Updater.Shutdown();
            Utility.Singleton<Utility.Log>.Instance.RecordEvent -= this._LogRecorder.Record;
            AppDomain.CurrentDomain.UnhandledException -= this.CurrentDomain_UnhandledException;
        }

        void Framework.IBootable.Launch()
        {
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
            this._LoadData();
            Utility.Singleton<Utility.Log>.Instance.RecordEvent += this._LogRecorder.Record;
            this._Updater.Add(this._Storage);
            this._ToConnectStorage(this._StorageUser);
        }

        private void _LoadData()
        {
            var entityGroupLayoutBuffer = System.IO.File.ReadAllBytes("entityGroupLayout.txt");
            var entityGroupLayouts = Utility.Serialization.Read<EntityGroupLayout[]>(entityGroupLayoutBuffer);
            Singleton<Resource>.Instance.EntityGroupLayouts = entityGroupLayouts;

            var buffer = System.IO.File.ReadAllBytes("entitys.txt");
            var entitys = Utility.Serialization.Read<EntityData[]>(buffer);
            Singleton<Resource>.Instance.Entitys = entitys;

            var skillBuffer = System.IO.File.ReadAllBytes("skills.txt");
            var skillDatas = Utility.Serialization.Read<SkillData[]>(skillBuffer);
            Singleton<Resource>.Instance.SkillDatas= skillDatas;

            var itemsBuffer = System.IO.File.ReadAllBytes("items.txt");
            var items = Utility.Serialization.Read<ItemPrototype[]>(itemsBuffer);
            Singleton<Resource>.Instance.Items = items;

            var itemFormulasBuffer = System.IO.File.ReadAllBytes("itemFormulas.txt");
            var itemFormulas = Utility.Serialization.Read<ItemFormula[]>(itemFormulasBuffer);
            Singleton<Resource>.Instance.Formulas = itemFormulas;
        }

        private void _BuildParams()
        {
            var config = new Utility.Ini(this._ReadConfig());

            this._StorageVerifyData.IPAddress = config.Read("Storage", "ipaddr");
            this._StorageVerifyData.Port = int.Parse(config.Read("Storage", "port"));
            this._StorageVerifyData.Account = config.Read("Storage", "account");
            this._StorageVerifyData.Password = config.Read("Storage", "password");

            
        }

        private void _BuildUser()
        {
            

            if (this._IsIpAddress(this._StorageVerifyData.IPAddress))
            {
                this._Storage = new Storage.User.Proxy(new Storage.User.RemotingFactory());
                this._StorageUser = this._Storage.SpawnUser("user");
            }
            else
            {
                var center = new Game.Storage.Center(new Game.DummyFrature());
                this._Updater.Add(center);
                var factory = new Storage.User.StandaloneFactory(center);
                this._Storage = new Storage.User.Proxy(factory);
                this._StorageUser = this._Storage.SpawnUser("user");
            }
        }

        private bool _IsIpAddress(string ip)
        {
            System.Net.IPAddress ipaddr;
            return System.Net.IPAddress.TryParse(ip, out ipaddr);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            this._LogRecorder.Record(ex.ToString());
            this._LogRecorder.Save();
        }

        private void _ToConnectStorage(Storage.User.IUser user)
        {
            var stage = new Storage.User.ConnectStorageStage(user, this._StorageVerifyData.IPAddress, this._StorageVerifyData.Port);
            stage.OnDoneEvent += this._ConnectResult;
            this._Machine.Push(stage);
        }

        private void _ConnectResult(bool result)
        {
            if (result)
            {
                this._ToVerifyStorage(this._StorageUser);
            }
            else
            {
                throw new SystemException("stroage connect fail");
            }
        }

        private void _ToVerifyStorage(Storage.User.IUser user)
        {
            var stage = new Storage.User.VerifyStorageStage(user, this._StorageVerifyData.Account, this._StorageVerifyData.Password);
            stage.OnDoneEvent += this._VerifyResult;
            this._Machine.Push(stage);
        }

        private void _VerifyResult(bool verify_result)
        {
            if (verify_result)
            {
                this._ToBuildClient();
            }
            else
            {
                throw new SystemException("stroage verify fail");
            }
        }

       

        

        private void _ToBuildClient()
        {
            var stage = new BuildCenterStage(this._StorageUser);

            stage.OnBuiledEvent += this._Play;

            this._Machine.Push(stage);
        }

        private void _Play(BuildCenterStage.ExternalFeature features)
        {
            this._Center = new Center(
                features.AccountFinder,                
                features.GameRecorder
                );

            this._Updater.Add(this._Center);
        }

        
        private string _ReadConfig()
        {
            return System.IO.File.ReadAllText("config.ini");
        }
    }
}
