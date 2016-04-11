using Regulus.CustomType;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public interface IMapFinder
    {
        IIndividual[] Find(Rect bound);
    }
}