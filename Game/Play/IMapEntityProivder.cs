using System.Collections.Generic;

using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public interface IMapEntityProivder
    {                
        
        IEnumerable<IIndividual> Supply(string name, Vector2 center, float direction);
    }
}