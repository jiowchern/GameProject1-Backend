using System;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class LoadRecordStage : IStage
    {
        private readonly Guid _AccountId;
        private readonly ISoulBinder _Binder;
        private readonly IGameRecorder _GameRecorder;

        public delegate void RecordCallback(GamePlayerRecord record);

        public event RecordCallback DoneEvent;

        public LoadRecordStage(Guid account_id , ISoulBinder binder, IGameRecorder gameRecorder)
        {
            _AccountId = account_id;
            _Binder = binder;
            _GameRecorder = gameRecorder;
        }

        void IStage.Enter()
        {
            _GameRecorder.Load(_AccountId).OnValue += _LoadResult; 
        }

        private void _LoadResult(GamePlayerRecord obj)
        {
            DoneEvent(obj);
        }

        void IStage.Leave()
        {
            
        }

        void IStage.Update()
        {

        }
    }
}