using System;

using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Wall : IVisible
    {
        private Guid _Id;
        private readonly Rect _Bounds;

        public Wall(Rect bounds)
        {
            this._Bounds = bounds;
            _Id = Guid.NewGuid();
        }

        System.Guid IVisible.Id
        {
            get { return _Id; }
        }

        CustomType.Rect IVisible.Bounds
        {
            get { return _Bounds; }
        }

        private System.Action _BoundsEvent;
        event System.Action IVisible.BoundsEvent
        {
            add { _BoundsEvent += value; }
            remove { _BoundsEvent -= value; }
        }
    }
}