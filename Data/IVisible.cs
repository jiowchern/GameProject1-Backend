using System;

using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    public interface IVisible
    {
        ENTITY EntityType { get; }
        Guid Id { get; }

        string Name { get; }

        //起始位置 , 速度 ,方向, 位移
        event Action<Vector2, float , float ,float> MoveEvent;

        Vector2 Position { get; }
    }
}