using Regulus.Project.ItIsNotAGame1.Game.Play;
using System;


namespace Regulus.Project.ItIsNotAGame1.Play
{
    public class Server : Regulus.Remoting.ICore
    {
        private readonly Regulus.Utility.LogFileRecorder _LogRecorder;

        private readonly Regulus.Utility.StageMachine _Machine;

        private readonly Regulus.Utility.Updater _Updater;

        private Regulus.Project.ItIsNotAGame1.Game.Play.Center _Center;

        private Regulus.Project.ItIsNotAGame1.Storage.User.Proxy _Storage;

        private Regulus.Project.ItIsNotAGame1.Storage.User.IUser _StorageUser;

        private Regulus.CustomType.Verify _StorageVerifyData;

        public Server()
        {
            _LogRecorder = new Regulus.Utility.LogFileRecorder("Play");

            _StorageVerifyData = new Regulus.CustomType.Verify();
            
            _Machine = new Regulus.Utility.StageMachine();
            _Updater = new Regulus.Utility.Updater();

            _BuildParams();
            _BuildUser();
        }

        void Regulus.Remoting.ICore.AssignBinder(Regulus.Remoting.ISoulBinder binder)
        {
            _Center.Join(binder);
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Working();
            _Machine.Update();
            return true;
        }

        void Regulus.Framework.IBootable.Shutdown()
        {
            _Updater.Shutdown();
            Regulus.Utility.Singleton<Regulus.Utility.Log>.Instance.RecordEvent -= _LogRecorder.Record;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        }

        void Regulus.Framework.IBootable.Launch()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Regulus.Utility.Singleton<Regulus.Utility.Log>.Instance.RecordEvent += _LogRecorder.Record;
            _Updater.Add(_Storage);            
            _ToConnectStorage(_StorageUser);
        }

        private void _BuildParams()
        {
            var config = new Regulus.Utility.Ini(_ReadConfig());

            _StorageVerifyData.IPAddress = config.Read("Storage", "ipaddr");
            _StorageVerifyData.Port = int.Parse(config.Read("Storage", "port"));
            _StorageVerifyData.Account = config.Read("Storage", "account");
            _StorageVerifyData.Password = config.Read("Storage", "password");

            
        }

        private void _BuildUser()
        {
            

            if (_IsIpAddress(_StorageVerifyData.IPAddress))
            {
                _Storage = new Regulus.Project.ItIsNotAGame1.Storage.User.Proxy(new Regulus.Project.ItIsNotAGame1.Storage.User.RemotingFactory());
                _StorageUser = _Storage.SpawnUser("user");
            }
            else
            {
                var center = new Regulus.Project.ItIsNotAGame1.Game.Storage.Center(new Regulus.Project.ItIsNotAGame1.Game.DummyFrature());
                _Updater.Add(center);
                var factory = new Regulus.Project.ItIsNotAGame1.Storage.User.StandaloneFactory(center);
                _Storage = new Regulus.Project.ItIsNotAGame1.Storage.User.Proxy(factory);
                _StorageUser = _Storage.SpawnUser("user");
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
            _LogRecorder.Record(ex.ToString());
            _LogRecorder.Save();
        }

        private void _ToConnectStorage(Regulus.Project.ItIsNotAGame1.Storage.User.IUser user)
        {
            var stage = new Regulus.Project.ItIsNotAGame1.Storage.User.ConnectStorageStage(user, _StorageVerifyData.IPAddress, _StorageVerifyData.Port);
            stage.OnDoneEvent += _ConnectResult;
            _Machine.Push(stage);
        }

        private void _ConnectResult(bool result)
        {
            if (result)
            {
                _ToVerifyStorage(_StorageUser);
            }
            else
            {
                throw new SystemException("stroage connect fail");
            }
        }

        private void _ToVerifyStorage(Regulus.Project.ItIsNotAGame1.Storage.User.IUser user)
        {
            var stage = new Regulus.Project.ItIsNotAGame1.Storage.User.VerifyStorageStage(user, _StorageVerifyData.Account, _StorageVerifyData.Password);
            stage.OnDoneEvent += _VerifyResult;
            _Machine.Push(stage);
        }

        private void _VerifyResult(bool verify_result)
        {
            if (verify_result)
            {
                _ToBuildClient();
            }
            else
            {
                throw new SystemException("stroage verify fail");
            }
        }

       

        

        private void _ToBuildClient()
        {
            var stage = new BuildCenterStage(_StorageUser);

            stage.OnBuiledEvent += _Play;

            _Machine.Push(stage);
        }

        private void _Play(BuildCenterStage.ExternalFeature features)
        {
            _Center = new Center(
                features.AccountFinder,                
                features.GameRecorder
                );

            _Updater.Add(_Center);
        }

        
        private string _ReadConfig()
        {
            return System.IO.File.ReadAllText("config.ini");
        }
    }
}
