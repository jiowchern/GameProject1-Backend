using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

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
        

        private readonly Dictionary<MAZEWALL ,WallKind > _WallToEntity;

        private readonly Regulus.Utility.IRandom _Random;
        public Realm(Regulus.Utility.IRandom rnd) :this()
        {
            _Random = rnd;
        }
        public Realm()
        {
            if(_Random == null)
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

            this._Map = this._BuildMap();

        }

        private Map _BuildMap()
        {
            var map = new Map();

            var rooms = new List<MazeCell>();
            var ailses = new List<MazeCell>();
            _BuildWall(map , ref rooms , ref ailses);
            _BuildResource(map, ailses);            
            _BuildGate(map, rooms);

            var emptyRooms = new Queue<MazeCell>(rooms.Shuffle());
            _BuildEnterance(map, emptyRooms);
            _BuildChest(map, emptyRooms);

            this._BuildDebirs(map);
            return map;
        }

        

        private void _BuildResource(Map map, List<MazeCell> ailses)
        {
            
            var random = Regulus.Utility.Random.Instance;
            foreach (var mazeCell in ailses)
            {
                var val = random.NextDouble();
                if (val > 0.5)
                    continue;


                var center = _GetCellPosition(mazeCell);
                                
                var individuals = _BuildEntityGroup("pool" , mazeCell, center);
                foreach (var individual in individuals)
                {
                    map.JoinStaff(individual);
                }

            }
        }

        private IEnumerable<IIndividual> _BuildEntityGroup(string id , MazeCell maze_cell, Vector2 center)
        {
            IEnumerable<IIndividual> individuals = new IIndividual[0];
            if (maze_cell.Walls[MAZEWALL.WESTERN])
            {
                individuals = _BuildEntityGroup(id, 180, center);
            }
            else if (maze_cell.Walls[MAZEWALL.NORTH])
            {
                individuals = _BuildEntityGroup(id, 90, center);
            }
            else if (maze_cell.Walls[MAZEWALL.EAST])
            {
                individuals = _BuildEntityGroup(id, 0, center);
            }
            else if (maze_cell.Walls[MAZEWALL.SOUTH])
            {
                individuals = _BuildEntityGroup(id, 270, center);
            }
            return individuals;
        }

        private void _BuildGate(Map map, IEnumerable<MazeCell> rooms)
        {
            foreach (var room in rooms)
            {

                var center = _GetCellPosition(room);

                IEnumerable<IIndividual> individuals = new IIndividual[0];
                if (room.Walls[MAZEWALL.WESTERN] == false)
                {
                    individuals = _BuildEntityGroup("gate", 0, center);
                }
                else if (room.Walls[MAZEWALL.NORTH] == false)
                {
                    individuals = _BuildEntityGroup("gate", 270, center);
                }
                else if (room.Walls[MAZEWALL.EAST] == false)
                {
                    individuals = _BuildEntityGroup("gate", 180, center);
                }
                else if (room.Walls[MAZEWALL.SOUTH] == false)
                {
                    individuals = _BuildEntityGroup("gate", 90, center);
                }

                foreach (var individual in individuals)
                {
                    map.JoinStaff(individual);
                }


            }
        }

        private IEnumerable<IIndividual> _BuildEntityGroup(string id,float degree, Vector2 center)
        {
            
            List<IIndividual> individuals = new List<IIndividual>();
            var layout = _GetGroupLayout(id);
            var buildInfos = from e in layout.Entitys 
                let radians = degree * (float)Math.PI / 180f 
                let position = Polygon.RotatePoint(e.Position , new Vector2(), radians)
                select new
                {
                    EntityType = e.Type,
                    Position = position + center,
                    Direction = e.Direction + degree
                };


            foreach (var info in buildInfos)
            {                
                IIndividual individual = _GetResourctEntity(info.EntityType);
                individual.SetPosition(info.Position);
                individual.AddDirection(degree);
                individuals.Add(individual);
            }
            return individuals;
        }

        private EntityGroupLayout _GetGroupLayout(string name)
        {
            return Resource.Instance.FindEntityGroupLayout(name);
        }
        

        private Vector2 _GetCellPosition(MazeCell cell)
        {
            var x = cell.Row * _Witdh;
            var y = cell.Column * _Height;
            return new Vector2(x,y);
        }
        private void _BuildChest(Map map, Queue<MazeCell> empty_rooms)
        {
            while (empty_rooms.Count > 0)
            {
                var room = empty_rooms.Dequeue();

            }
        }

        private void _BuildEnterance(Map map , Queue<MazeCell> rooms)
        {
            var queue = rooms;

            this._BuildPlayerEnternace(map , queue);

            this._BuildAboriginalEnternace(map , queue);
        }

        private void _BuildAboriginalEnternace(Map map, Queue<MazeCell> positions)
        {


            var len = Maze.kDimension / 3;

            for (int i = 0 ; i < len && positions.Any(); ++i)
            {
                this._BuildEnternace(map, new []{ENTITY.ACTOR2, ENTITY.ACTOR3 }, positions.Dequeue());
            }

            for (int i = 0; i < len && positions.Any(); ++i)
            {
                this._BuildEnternace(map, new[] { ENTITY.ACTOR2, ENTITY.ACTOR4 }, positions.Dequeue());
            }

            for (int i = 0; i < len && positions.Any(); ++i)
            {
                this._BuildEnternace(map, new[] { ENTITY.ACTOR2, ENTITY.ACTOR5 }, positions.Dequeue());
            }



        }

        private void _BuildPlayerEnternace(Map map , Queue<MazeCell> positions)
        {

            var types = new[]
                {
                    ENTITY.ACTOR1 , ENTITY.ACTOR2 , ENTITY.DEBIRS //, ENTITY.ACTOR3 , ENTITY.ACTOR4 , ENTITY.ACTOR5
                };            
            
            _BuildEnternace(map, types, positions.Dequeue());
        }
        
        private void _BuildEnternace(Map map, ENTITY[] types , MazeCell cell)
        {
            var entity = EntityProvider.CreateEnterance(types);
            IIndividual individual = entity;

            var position = _GetCellPosition(cell);            

            individual.SetPosition(position.X, position.Y);

            map.JoinStaff(individual);
        }

        private void _BuildWall(Map map , ref List<MazeCell> rooms , ref List<MazeCell> aisles)
        {
            IEnumerable<MazeCell> cells = this._BuildMaze();
            foreach(var cell in cells)
            {
                if (cell.Walls.Count() >= 3)
                {
                    rooms.Add(cell);
                }
                else
                {
                    aisles.Add(cell);
                }

                foreach (var wall in cell.Walls)
                {                                        
                    IIndividual entity = this._BuildWall(wall, cell.Row, cell.Column, true);
                    map.JoinStaff(entity);
                }
            }
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

        private IIndividual _BuildWall(MAZEWALL wall, int row, int column , bool room)
        {
            Entity entity = this._GetEntity(wall , room);
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

        private Entity _GetEntity(MAZEWALL wall , bool room)
        {                        
            var entity  = this._WallToEntity[wall].Get(room);
            return _GetEntity(entity);
        }

        private Entity _GetResourctEntity(ENTITY type)
        {
            var entity = EntityProvider.CreateResource(type, new ResourceInventory());
            return entity;

        }
        private Entity _GetEntity(ENTITY type)
        {            
            var entity = EntityProvider.Create(type);            
            return entity;
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