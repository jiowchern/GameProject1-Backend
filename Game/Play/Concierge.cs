
using System.Linq;

using Regulus.CustomType;
using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Concierge
    {
        private readonly Polygon _Body;

        private readonly ENTITY[] _AcceptsEntitys;

        public Concierge(Polygon body , ENTITY[] accepts_entitys)
        {
            this._Body = body;
            this._AcceptsEntitys = accepts_entitys;
        }

        public bool IsAcceptsType(IIndividual individual)
        {
            return _AcceptsEntitys.Any( e=> e == individual.EntityType);
        }

        public Vector2 GetPosition()
        {
            return _Body.Center;
        }
    }
}