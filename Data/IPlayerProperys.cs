using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    public interface IPlayerProperys
    {
        Guid Id { get; }

        float Strength { get; }
    }
}
