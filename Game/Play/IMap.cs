using System;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal interface IMap
    {
        void Join(IVisible observable, Regulus.Project.ItIsNotAGame1.Data.Location location);
        void Left(IVisible observable);
        IVisible[] Find(IObservable observable);

        event Action<IVisible, Regulus.Project.ItIsNotAGame1.Data.Location> ChangeEvent;
    }
}