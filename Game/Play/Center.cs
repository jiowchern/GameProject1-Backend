using System;
using System.Security.Policy;

using Regulus.Framework;
using Regulus.Game;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Center : IUpdatable , Remoting.ICore
    {
        private readonly IAccountFinder _AccountFinder;
        private readonly IGameRecorder _GameRecorder;
        private readonly Hall _Hall;
        private readonly Updater _Updater;

        private readonly Race _Race;

        private Zone _Zone;
        public Center(IAccountFinder account_finder, IGameRecorder game_recorder  )
        {
            
            this._AccountFinder = account_finder;
            this._GameRecorder = game_recorder;
            this._Hall = new Hall();
            this._Updater = new Updater();
            this._Zone = new Zone(new [] { new RealmInfomation { Name = "test", EntityDatas = new EntityData[0] , Dimension = 20} });

            _Race = new Race(_Zone);
        }
        public void Join(Remoting.ISoulBinder binder)
        {
            this._Hall.PushUser(new User(binder , this._AccountFinder , this._GameRecorder , this._Zone));
        }

        void IBootable.Launch()
        {
            this._Updater.Add(this._Hall);
            this._Updater.Add(this._Zone);
            _Updater.Add(_Race);
        }

        void IBootable.Shutdown()
        {
            this._Updater.Shutdown();
        }

        bool IUpdatable.Update()
        {
            this._Updater.Working();
            return true;
        }

        void Remoting.ICore.AssignBinder(Remoting.ISoulBinder binder)
        {
            this.Join(binder);
        }
    }
}