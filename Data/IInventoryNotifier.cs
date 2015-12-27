using System;

using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    public interface IInventoryNotifier
    {
        void Query();

        void Discard(Guid id);

        event Action<Item[]> AllItemEvent;
        event Action<Item> AddEvent;
        event Action<Guid> RemoveEvent;
    }
}