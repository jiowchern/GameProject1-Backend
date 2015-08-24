using System;

using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    public interface IVisible
    {
        Guid Id { get; }

        string Name { get; }

        event Action<Vector2, Vector2> MoveEvent;

        Vector2 Position { get; }
    }
}