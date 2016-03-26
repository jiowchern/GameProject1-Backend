using System;
using System.Collections.Generic;

using Regulus.CustomType;

using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Realm
    {        
        private Map _Map;

        private readonly float _Witdh;

        private readonly float _Height;


        class WallShift
        {
            private readonly Vector2 _Offset;

            private readonly Func<Rect, Vector2, Vector2> _ShiftHandler;

            public WallShift(Vector2 offset , Func<Rect , Vector2 , Vector2> shift_handler)
            {
                this._Offset = offset;
                this._ShiftHandler = shift_handler;                
            }

            public static Vector2 _East(Rect bound, Vector2 offset)
            {                
                return new Vector2(offset.X - bound.Width/ 2, offset.Y);
            }

            public static Vector2 _South(Rect bound, Vector2 offset)
            {
                return new Vector2(offset.X , offset.Y + bound.Height / 2);
            }

            public static Vector2 _Western(Rect bound, Vector2 offset)
            {
                return new Vector2(offset.X + bound.Width / 2, offset.Y);
            }

            public static Vector2 _North(Rect bound, Vector2 offset)
            {
                return new Vector2(offset.X , offset.Y - bound.Height / 2);
            }

            public Vector2 GetDatum(Rect bound)
            {
                return this._ShiftHandler(bound , this._Offset);
            }
        }

        private readonly Dictionary<MAZEWALL, WallShift> _DirectionPoints;
        

        private readonly Dictionary<MAZEWALL ,WallKind > _WallToEntity;

        private readonly Regulus.Utility.IRandom _Random;

        private readonly int _Dimension;

        public Realm(int dimension , Regulus.Utility.IRandom rnd) :this(dimension)
        {
            
            _Random = rnd;
        }
        public Realm(int dimension)
        {
            _Dimension = dimension;
            if (_Random == null)
                _Random = Regulus.Utility.Random.Instance;
            this._WallToEntity = new Dictionary<MAZEWALL, WallKind>();
            this._WallToEntity.Add(MAZEWALL.EAST, new WallKind(ENTITY.WALL_EAST , ENTITY.WALL_EAST_AISLE) );
            this._WallToEntity.Add(MAZEWALL.SOUTH, new WallKind(ENTITY.WALL_SOUTH , ENTITY.WALL_SOUTH_AISLE ));
            this._WallToEntity.Add(MAZEWALL.WESTERN, new WallKind(ENTITY.WALL_WESTERN , ENTITY.WALL_WESTERN_AISLE) );
            this._WallToEntity.Add(MAZEWALL.NORTH, new WallKind(ENTITY.WALL_NORTH , ENTITY.WALL_NORTH_AISLE));

            this._Witdh = 15    ;
            this._Height = 15;

            this._DirectionPoints = new Dictionary<MAZEWALL, WallShift>();
            this._DirectionPoints.Add(MAZEWALL.EAST, new WallShift(new Vector2(this._Witdh / 2  , 0) , WallShift._East));
            this._DirectionPoints.Add(MAZEWALL.SOUTH, new WallShift(new Vector2(0, -this._Height / 2), WallShift._South));
            this._DirectionPoints.Add(MAZEWALL.WESTERN, new WallShift(new Vector2(-this._Witdh/ 2, 0), WallShift._Western));
            this._DirectionPoints.Add(MAZEWALL.NORTH, new WallShift(new Vector2(0, this._Height / 2), WallShift._North));

            

        }

        

        

        
        

        private Vector2 _GetCellPosition(MazeCell cell)
        {
            var x = cell.Row * _Witdh;
            var y = cell.Column * _Height;
            return new Vector2(x,y);
        }
        

        

        

        
        
        

        

        private void _BuildDebirs(Map map)
        {            
            for(int i = 0; i < 20; ++ i )
            {
                var entity = this._GetEntity(ENTITY.DEBIRS);                
                map.JoinChallenger(entity);
                IIndividual individual = entity;
                individual.AddDirection(Utility.Random.Instance.NextFloat(0, 360));

                var x = individual.Position.X;
                var y = individual.Position.Y;

                individual.SetPosition(x + Utility.Random.Instance.NextFloat(0, 20), y + Utility.Random.Instance.NextFloat(0, 20));
                
                
            }
        }

        

        

        
        private Entity _GetEntity(ENTITY type)
        {            
            var entity = EntityProvider.Create(type);            
            return entity;
        }


        internal static IEnumerable<MazeCell> BuildMaze(int dimension)
        {
            return _BuildMaze(dimension);
        }

        private IEnumerable<MazeCell> _BuildMaze()
        {
            return _BuildMaze(_Dimension);
        }
        private static IEnumerable<MazeCell> _BuildMaze(int dimension)
        {
            var maze = new Maze(dimension);
            maze.Generate();
            int index = 0;
            
            foreach(var cell in maze.Cells)
            {
                var walls = new Flag<MAZEWALL>();
                int column = cell.Column;
                int row = cell.Row;
                if(cell.Walls[0] == 1)
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

        public Map Query()
        {
            return this._Map;
        }

        public Map NewMap(IMapEntityProivder provider)
        {
            var builder = new LevelGenerator(provider);
            var map = builder.Build(_Witdh , _Height , _Dimension);
            _Map =  map;
            return _Map;
        }
    }
}