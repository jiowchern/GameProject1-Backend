using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    public interface IEquipmentNotifier
    {
        event Action<Item> AddEvent;
        event Action<Guid> RemoveEvent;
        event Action<Item[]> FlushEvent;

        void Query();

        void Unequip(Guid id);        
    }
}
