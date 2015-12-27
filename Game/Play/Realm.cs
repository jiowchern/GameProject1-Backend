using System;
using System.Collections.Generic;

using Regulus.CustomType;
using Regulus.Extension;
using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Realm
    {        
        private readonly Map _Map;

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
        

        private readonly Dictionary<MAZEWALL ,ENTITY> _WallToEntity;

        public Realm()
        {
            this._WallToEntity = new Dictionary<MAZEWALL, ENTITY>();
            this._WallToEntity.Add(MAZEWALL.EAST, ENTITY.WALL_EAST);
            this._WallToEntity.Add(MAZEWALL.SOUTH, ENTITY.WALL_SOUTH);
            this._WallToEntity.Add(MAZEWALL.WESTERN, ENTITY.WALL_WESTERN);
            this._WallToEntity.Add(MAZEWALL.NORTH, ENTITY.WALL_NORTH);

            this._Witdh = 10;
            this._Height = 10;

            this._DirectionPoints = new Dictionary<MAZEWALL, WallShift>();
            this._DirectionPoints.Add(MAZEWALL.EAST, new WallShift(new Vector2(this._Witdh / 2  , 0) , WallShift._East));
            this._DirectionPoints.Add(MAZEWALL.SOUTH, new WallShift(new Vector2(0, -this._Height / 2), WallShift._South));
            this._DirectionPoints.Add(MAZEWALL.WESTERN, new WallShift(new Vector2(-this._Witdh/ 2, 0), WallShift._Western));
            this._DirectionPoints.Add(MAZEWALL.NORTH, new WallShift(new Vector2(0, this._Height / 2), WallShift._North));

            this._Map = this._BuildMap();

        }

        private Map _BuildMap()
        {
            var map = new Map();
            IEnumerable<MazeCell> cells = this._BuildMaze();
            foreach(var cell in cells)
            {
                foreach(var wall in cell.Walls)
                {
                    IIndividual entity = this._BuildWall(wall , cell.Row , cell.Column  );
                    map.JoinStaff(entity);                    
                }
            }

            this._BuildDebirs(map);
            return map;
        }

        private void _BuildDebirs(Map map)
        {
            
            for(int i = 0; i < 20; ++ i )
            {
                var entity = this._GetEntity(ENTITY.DEBIRS);
                IIndividual individual = entity;
                individual.SetPosition(Utility.Random.Instance.NextFloat(0, 100), Utility.Random.Instance.NextFloat(0, 100));
                map.JoinStaff(entity);
            }
            
           
        }

        private IIndividual _BuildWall(MAZEWALL wall, int row, int column)
        {
            Entity entity = this._GetEntity(wall);
            IIndividual individual = entity;

            var bound = individual.Mesh.Points.ToRect();

            Vector2 offset = this._GetOffset(wall , -row * this._Witdh,- column * this._Height , bound);

            individual.SetPosition(-offset.X , -offset.Y);
            return entity;
        }

        private Vector2 _GetOffset(MAZEWALL wall, float center_x, float center_y, Rect bound)
        {
            Vector2 directPoint = this._DirectionPoints[wall].GetDatum(bound);
            return new Vector2(directPoint.X + center_x , directPoint.Y + center_y);
        }

        private Entity _GetEntity(MAZEWALL wall)
        {            
            var record = new GamePlayerRecord();
            record.Entity = this._WallToEntity[wall];
            return EntityProvider.Create(record);
        }

        private Entity _GetEntity(ENTITY entity)
        {
            var record = new GamePlayerRecord();
            record.Entity = entity;
            return EntityProvider.Create(record);
        }

        private IEnumerable<MazeCell> _BuildMaze()
        {
            var maze = new Maze();
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
    }
}