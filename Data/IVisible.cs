using System;

using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    public interface IVisible
    {
        ENTITY EntityType { get; }
        Guid Id { get; }

        string Name { get; }

        float View { get; }

        event Action<EquipStatus[]> EquipEvent;

        //狀態 , 起始位置 , 速度 ,方向, 位移
        event Action<VisibleStatus> StatusEvent;
        
        Vector2 Position { get; }

        void QueryStatus();

        event Action<string> TalkMessageEvent;
    }
}