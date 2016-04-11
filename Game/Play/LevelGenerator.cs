using System;
using System.Collections.Generic;
using System.Linq;

using Regulus.CustomType;
using Regulus.Extension;


namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class LevelGenerator
    {
        

        private readonly float _Width;

        private readonly float _Height;

        private readonly Level _Level;

        public LevelGenerator(float witdh, float height)
        {
            _Level = new Level();
            _Width = witdh;
            _Height = height;
        }
        
        public Level Build(int dimension)
        {
            
            var mapCells = LevelGenerator._BuildMaze(dimension);
            var rooms = new List<MazeCell>();
            var aisles = new List<MazeCell>();
            foreach (var cell in mapCells)
            {                
                var walls = cell.Walls;
                var center = cell.GetPosition(_Width, _Height);
                foreach (var wall in walls)
                {
                    var dir = _WallToDirection(wall);
                    _Level.Add(new LevelUnit(LEVEL_UNIT.WALL , center, dir));                    
                }

                if (cell.IsRoom())
                {
                    rooms.Add(cell);
                }
                else if (cell.HaveWall())
                {                    
                    aisles.Add(cell);
                }
            }

            _BuildScene(rooms.Shuffle().ToArray(), aisles.Shuffle().ToArray());

            return _Level;
        }

        private IEnumerable<float> _GetWallDirection(Flag<MAZEWALL> walls)
        {
            var againstWall = (from wall in walls select wall).Shuffle().First();
            var direction = _WallToDirection(againstWall);
            return new[] { direction } ;
        }

        private IEnumerable<float> _GetExitDirection(Flag<MAZEWALL> walls)
        {
            var exit = (from wall in Regulus.Utility.EnumHelper.GetEnums<MAZEWALL>()
                        where walls[wall] == false
                        select wall).Single();
            var exitDirection = _WallToDirection(exit);
            return new[] { exitDirection } ;
        }

        private void _BuildScene(IEnumerable<MazeCell> rooms, IEnumerable<MazeCell> aisles)
        {
            var roomCount = rooms.Count();
            var enteranceEnd0 = 0;
            var enteranceEnd1 = (int)(roomCount * 0.1);
            var enteranceEnd2 = (int)(roomCount * 0.2) + enteranceEnd1;
            var enteranceEnd3 = (int)(roomCount * 0.2) + enteranceEnd2;
            var enteranceEnd4 = (int)(roomCount * 0.2) + enteranceEnd3;
            var chestEnd = (int)(roomCount * 0.3) + enteranceEnd4;

            _Build(LEVEL_UNIT.ENTERANCE1, rooms.ToArray().Skip(enteranceEnd0).Take(enteranceEnd1 - enteranceEnd0), _GetExitDirection);
            _Build(LEVEL_UNIT.ENTERANCE2, rooms.ToArray().Skip(enteranceEnd1).Take(enteranceEnd2 - enteranceEnd1), _GetExitDirection);
            _Build(LEVEL_UNIT.ENTERANCE3, rooms.ToArray().Skip(enteranceEnd2).Take(enteranceEnd3 - enteranceEnd2), _GetExitDirection);
            _Build(LEVEL_UNIT.ENTERANCE4, rooms.ToArray().Skip(enteranceEnd3).Take(enteranceEnd4 - enteranceEnd3), _GetExitDirection);
            _Build(LEVEL_UNIT.CHEST, rooms.ToArray().Skip(enteranceEnd4).Take(chestEnd), _GetExitDirection);


            var aisleCount = aisles.Count();
            
            var thickWallCount = (int)(aisleCount * 0.4f);
            var thickWallCells = aisles.ToArray().Skip(0).Take(thickWallCount);
            _Build(LEVEL_UNIT.GATE, thickWallCells, _GetWallDirections);

            var poolCount = (int)(aisleCount * 0.2f);
            var poolCells = aisles.ToArray().Skip(thickWallCount).Take(poolCount);
            _Build(LEVEL_UNIT.POOL, poolCells, _GetWallDirection);

            var fieldsCount = (int)(aisleCount * 0.4f);
            var fieldsCells = aisles.ToArray().Skip(thickWallCount+ poolCount).Take(fieldsCount);
            _Build(LEVEL_UNIT.FIELD, fieldsCells, _GetWallDirection);

        }

        private IEnumerable<float> _GetWallDirections(Flag<MAZEWALL> walls)
        {
            foreach (var mazewall in walls)
            {
                yield return _WallToDirection(mazewall);
            }            
        }

        private void _Build(LEVEL_UNIT unit, IEnumerable<MazeCell> cells , Func<Flag<MAZEWALL>,IEnumerable<float>> get_directions )
        {
            foreach (var mazeCell in cells)
            {
                var center = mazeCell.GetPosition(_Width, _Height);
                foreach (var direction in get_directions(mazeCell.Walls))
                {
                    _Level.Add(new LevelUnit(unit , center , direction));
                }
                
            }            
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

        private static IEnumerable<MazeCell> _BuildMaze(int dimension)
        {
            var maze = new Maze(dimension);
            maze.Generate();
            int index = 0;

            foreach (var cell in maze.Cells)
            {
                var walls = new Flag<MAZEWALL>();
                int column = cell.Column;
                int row = cell.Row;
                if (cell.Walls[0] == 1)
                {
                    walls[MAZEWALL.NORTH] = true;
                }
                if (cell.Walls[1] == 1)
                {
                    walls[MAZEWALL.EAST] = true;

                }
                if (cell.Walls[2] == 1)
                {
                    walls[MAZEWALL.SOUTH] = true;
                }
                if (cell.Walls[3] == 1)
                {
                    walls[MAZEWALL.WESTERN] = true;
                }
                yield return new MazeCell()
                {
                    Row = row,
                    Column = column,
                    Walls = walls
                };

                index++;
            }
        }
    }



}