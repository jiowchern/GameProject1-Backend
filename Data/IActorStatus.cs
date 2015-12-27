using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    public interface IActorStatus
    {
        Guid Id { get; }
        event Action<ACTOR_STATUS_TYPE, ACTOR_STATUS_TYPE> ChangeEvent;
    }
}
