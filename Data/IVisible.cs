using System;

using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    public interface IVisible
    {
        ENTITY EntityType { get; }
        Guid Id { get; }

        string Name { get; }

        //�_�l��m , �t�� ,��V, �첾
        event Action<Vector2, float , float ,float> MoveEvent;

        Vector2 Position { get; }
    }
}