using System.Collections.Generic;

using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public interface IMapEntityProivder
    {        
        IEnumerable<IIndividual> NewRoom(Vector2 center, float direction);

        IEnumerable<IIndividual> NewEnterance(Vector2 center, float direction);

        IEnumerable<IIndividual> NewResource(Vector2 center, float direction);        

        IEnumerable<IIndividual> NewWall(Vector2 center, float direction);
    }
}