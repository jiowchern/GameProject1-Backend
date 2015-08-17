using System;

using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public interface IVisible
    {
        Guid Id { get; }
        Rect Bounds { get; }

        event Action BoundsEvent;
    }
}