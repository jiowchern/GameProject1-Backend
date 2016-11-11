using System.Collections.Generic;
using System.Linq;
using Regulus.CustomType;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class EntityGroupBuilder
    {
        private readonly string _Id;

        private readonly OnCreateInteractive _CreateInteractive;
        internal EntityGroupBuilder(string id , OnCreateInteractive create_interactive)
        {
            _CreateInteractive = create_interactive;
            _Id = id;
        }

        internal delegate IUpdatable OnCreateInteractive(Entity[] entitys, IMapGate gate , IMapFinder finder);
        internal IUpdatable Create(float degree, Vector2 center , IMapGate gate, IMapFinder finder)
        {
            var entitys = _CreateFromGroup(_Id, degree, center);
            return _CreateInteractive(entitys.ToArray() , gate , finder);            
        }        
        
        private IEnumerable<Entity> _CreateFromGroup(string id, float degree, Vector2 center)
        {
            
            var layout = Resource.Instance.FindEntityGroupLayout(id);
            yield return from e in layout.Entitys
                             let radians = degree * (float)System.Math.PI / 180f
                             let position = Polygon.RotatePoint(e.Position, new Vector2(), radians)
                             select EntityProvider.Create(e.Build());            
            
        }
    }
}