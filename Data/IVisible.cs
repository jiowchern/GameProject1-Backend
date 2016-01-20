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

        //���A , �_�l��m , �t�� ,��V, �첾
        event Action<VisibleStatus> StatusEvent;
        
        Vector2 Position { get; }

        void QueryStatus();

        event Action<string> TalkMessageEvent;
    }
}