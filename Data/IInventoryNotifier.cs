using System;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    public interface IInventoryNotifier
    {        
        event Action<Item> AddEvent;
        event Action<Guid> RemoveEvent;
    }
}