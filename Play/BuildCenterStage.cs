using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Project.ItIsNotAGame1.Storage.User;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Play
{
    internal class BuildCenterStage : IStage
    {
        public delegate void SuccessBuiledCallback(ExternalFeature features);

        public event SuccessBuiledCallback OnBuiledEvent;

        public struct ExternalFeature
        {
            public IAccountFinder AccountFinder;

            

            public IGameRecorder GameRecorder;




        }

        

        private readonly IUser _StorageUser;

        private ExternalFeature _Feature;

        public BuildCenterStage( IUser storage_user)
        {
            _Feature = new ExternalFeature();
            
            _StorageUser = storage_user;
        }

        void IStage.Enter()
        {
            _StorageUser.QueryProvider<IAccountFinder>().Supply += _AccountFinder;
        }

        void IStage.Leave()
        {
        }

        void IStage.Update()
        {
        }

        

        private void _AccountFinder(IAccountFinder obj)
        {
            _StorageUser.QueryProvider<IAccountFinder>().Supply -= _AccountFinder;
            _Feature.AccountFinder = obj;

            _StorageUser.QueryProvider<IGameRecorder>().Supply += _RecordQueriers;
        }

        
        private void _RecordQueriers(IGameRecorder obj)
        {
            _StorageUser.QueryProvider<IGameRecorder>().Supply -= _RecordQueriers;
            _Feature.GameRecorder = obj;

            OnBuiledEvent(_Feature);
        }

       
    }
}