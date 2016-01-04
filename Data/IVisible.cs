using System;

using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    [ProtoBuf.ProtoContract]
    public struct VisibleStatus
    {
        [ProtoBuf.ProtoMember(1)]
        public ACTOR_STATUS_TYPE Status { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public Vector2 StartPosition { get; set; }
        [ProtoBuf.ProtoMember(3)]
        public float Speed { get; set; }
        [ProtoBuf.ProtoMember(4)]
        public float Direction { get; set; }
        [ProtoBuf.ProtoMember(5)]
        public float Trun { get; set; }
    }
    public interface IVisible
    {
        ENTITY EntityType { get; }
        Guid Id { get; }

        string Name { get; }
        
        //狀態 , 起始位置 , 速度 ,方向, 位移
        event Action<VisibleStatus> StatusEvent;
        
        Vector2 Position { get; }

        void QueryStatus();

        event Action<string> TalkMessageEvent;
    }
}