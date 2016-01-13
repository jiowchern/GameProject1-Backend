using System;

using Regulus.CustomType;
using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public interface IIndividual : IVisible
    {
        Rect Bounds { get; }        

        Polygon Mesh { get; }

        event Action BoundsEvent;

        void SetPosition(float x, float y);

        Item[] Stolen();

        void AttachDamage(bool smash);

        bool IsBlock();

        Concierge GetConcierge();

        void SetDirection(float dir);
    }
}