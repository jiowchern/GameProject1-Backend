using System.Security.Policy;

using Regulus.Framework;
using Regulus.Game;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Center : Regulus.Utility.IUpdatable , Regulus.Remoting.ICore
    {
        private readonly Regulus.Project.ItIsNotAGame1.Data.IAccountFinder _AccountFinder;
        private readonly Regulus.Project.ItIsNotAGame1.Data.IGameRecorder _GameRecorder;
        private readonly Regulus.Game.Hall _Hall;
        private readonly Regulus.Utility.Updater _Updater;

        private Regulus.Project.ItIsNotAGame1.Game.Play.Zone _Zone;
        public Center(IAccountFinder accountFinder, IGameRecorder gameRecorder  )
        {
            _AccountFinder = accountFinder;
            _GameRecorder = gameRecorder;
            _Hall = new Hall();            
            _Updater = new Updater();
            _Zone = new Zone(new RealmMaterial[] { new RealmMaterial { Name = "test", EntityDatas = new EntityData[0] } });
        }
        public void Join(Regulus.Remoting.ISoulBinder binder)
        {
            _Hall.PushUser(new User(binder , _AccountFinder , _GameRecorder , _Zone));
        }

        void IBootable.Launch()
        {
            _Updater.Add(_Hall);
            _Updater.Add(_Zone);
        }

        void IBootable.Shutdown()
        {
            _Updater.Shutdown();
        }

        bool IUpdatable.Update()
        {
            _Updater.Working();
            return true;
        }

        void Remoting.ICore.AssignBinder(Remoting.ISoulBinder binder)
        {
            Join(binder);
        }
    }
}