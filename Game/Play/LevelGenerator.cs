using System;

using System.Linq;


using Regulus.Extension;


namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class LevelGenerator
    {
        private readonly IMapEntityProivder _Provider;



        public LevelGenerator(IMapEntityProivder provider)
        {
            _Provider = provider;
        }

        public Map Build(float witdh, float height, int dimension)
        {
            var map = new Map();
            var mapCells = Realm.BuildMaze(dimension);
            foreach (var cell in mapCells)
            {
                var center = cell.GetPosition(witdh, height);
                var walls = cell.Walls;

                foreach (var wall in walls)
                {
                    var dir = _WallToDirection(wall);
                    map.JoinStaffs(_Provider.NewWall(center, dir));
                }

                if (cell.IsRoom())
                {
                    var exit = (from wall in Regulus.Utility.EnumHelper.GetEnums<MAZEWALL>()
                                where walls[wall] == false
                                select wall).Single();
                    var exitDirection = _WallToDirection(exit);                    
                    map.JoinStaffs(_Provider.NewEnterance(center, exitDirection));

                    
                }
                else if (cell.HaveWall())
                {
                    var againstWall = (from wall in walls select wall).Shuffle().First();
                    map.JoinStaffs(_Provider.NewResource(center, _WallToDirection(againstWall)));
                }
            }

            
            return map;
        }

        private float _WallToDirection(MAZEWALL wall)
        {
            if (wall == MAZEWALL.WESTERN)
            {
                return 0;
            }
            else if (wall == MAZEWALL.NORTH)
            {
                return 270;
            }
            else if (wall == MAZEWALL.EAST)
            {
                return 180;
            }
            else if (wall == MAZEWALL.SOUTH)
            {
                return 90;
            }

            throw new Exception("Can not identify type.");
        }
    }
}