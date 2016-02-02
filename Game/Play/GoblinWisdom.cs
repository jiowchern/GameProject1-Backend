using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;

using Regulus.BehaviourTree;
using Regulus.Extension;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Utility;


using Vector2 = Regulus.CustomType.Vector2;




namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class GoblinWisdom : Wisdom
    {
        private readonly Entity _Entiry;


        private List<Hit> _WallTargets;

        private List<IVisible> _Vision;
        private IMoveController _MoveController;

        private float _ScanCycle;

        private float _DecisionTime;
        



        private float _WallDistance;

        private float _ExitScanTimer;

        public GoblinWisdom(Entity entiry)
        {            
            _DecisionTime = 0.5f;
            
            _WallTargets = new List<Hit>();
            _Vision = new List<IVisible>();
                        
            _Entiry = entiry;
            var viewBound = _Entiry.GetView();
            var left = new Vector2(viewBound.X, viewBound.Y);
            var angle = Vector2.VectorToAngle((_Entiry.GetPosition() - left).GetNormalized());

            var rect = Resource.Instance.FindEntity((_Entiry as IVisible).EntityType).Mesh.Points.ToRect();
            _WallDistance = rect.Width > rect.Height
                ? rect.Width
                : rect.Height;
            
        }

        public float TurnDirection { get; set; }

        protected override Regulus.BehaviourTree.ITicker _Launch()
        {
            var viewLength = (_Entiry as IVisible).View;
            _Bind();
            var builder = new Regulus.BehaviourTree.Builder();
            var node = builder
                            .Selector()                                
                                // 近距離障礙物躲避策略
                                .Sequence()           
                                
                                    // 是否碰到障礙物
                                    .Action((delta) => _DetectObstacle(delta , _WallDistance  , _GetOffsetDirection(270)) )
                                    // 停止移動
                                    .Action(_StopMove)
                                    // 檢查周遭障礙物
                                    .Action(new ObstacleDetector(_DecisionTime , _Entiry , this , viewLength, 180 , true) )                                    
                                    // 旋轉至出口
                                    .Action(new TurnHandler(this) )
                                .End()
                                // 遠距離尋路策略
                                .Sequence()

                                    // 是否碰到障礙物
                                    .Action((delta) => _DetectObstacle(delta, viewLength / 2 , 0))
                                    // 停止移動
                                    .Action(_StopMove)
                                    // 檢查周遭障礙物
                                    .Action(new ObstacleDetector(_DecisionTime, _Entiry, this, viewLength , 180 , false))
                                    // 旋轉至出口
                                    .Action(new TurnHandler(this))
                                .End()

                                // 前進
                                .Sequence()
                                    .Action(_MoveForward)
                                .End()
                            .End()
                        .Build();
            return node;
        }

        private TICKRESULT _StopMove(float delta)
        {
            if (_MoveController != null)
            {
                
                _MoveController.StopMove();
                return TICKRESULT.SUCCESS;
            }
            
            return TICKRESULT.FAILURE;
        }

        private TICKRESULT _DetectObstacle(float delta, float distance , float angle)
        {
            
            var target = _Detect(angle + _Entiry.Direction, distance);

            if (target.Visible != null && IsWall(target.Visible.EntityType))
            {
                
                return TICKRESULT.SUCCESS;
            }
            
            return TICKRESULT.FAILURE;
        }
        
        private TICKRESULT _MoveForward(float arg1)
        {
            if (_MoveController != null)
            {
                _MoveController.Forward();
                return TICKRESULT.SUCCESS;
            }
            return TICKRESULT.FAILURE;
                
        }



        

        



        

        private Vector2 _CalcultedTurnForce(Hit tagret)
        {
            var pos = _Entiry.GetPosition();
            var vectorB = tagret.HitPoint - pos;
            return vectorB;
        }

        private void _Bind()
        {
            Transponder.Query<IVisible>().Supply += _SetIndividual;
            Transponder.Query<IVisible>().Unsupply += _ClearIndividual;
            Transponder.Query<IMoveController>().Supply += _SetMoveController;
            Transponder.Query<IMoveController>().Unsupply += _ClearMoveController;
        }
        private void _Unbind()
        {
            Transponder.Query<IMoveController>().Supply -= _SetMoveController;
            Transponder.Query<IMoveController>().Unsupply -= _ClearMoveController;
            Transponder.Query<IVisible>().Supply -= _SetIndividual;
            Transponder.Query<IVisible>().Unsupply -= _ClearIndividual;
        }

        private void _ClearIndividual(IVisible obj)
        {
            _Vision.RemoveAll(i => i.Id == obj.Id);
        }

        private void _SetIndividual(IVisible obj)
        {
            _Vision.Add(obj);
        }

        private void _ClearMoveController(IMoveController obj)
        {
            _MoveController = null;

        }

        private void _SetMoveController(IMoveController obj)
        {
            _MoveController = obj;

        }

        public struct Hit
        {
            public float Distance { get; set; }

            public IVisible Visible { get; set; }

            public Vector2 HitPoint { get; set; }
        }
        private Hit _Detect(float scan_angle, float max_distance)
        {
            scan_angle += 360f;
            scan_angle %= 360f;
            var pos = _Entiry.GetPosition();
            var view = Vector2.AngleToVector(scan_angle);

            // UnityEngine.Debug.DrawRay(new UnityEngine.Vector3(pos.X, 0, pos.Y), new UnityEngine.Vector3(view.X, 0, view.Y), UnityEngine.Color.blue, 0.5f);

            var hit = new Hit();
            hit.HitPoint = pos + view * max_distance;
            hit.Distance = max_distance;
            foreach (var visible in _Vision)
            {
                var data = Resource.Instance.FindEntity(visible.EntityType);
                var mesh = data.Mesh.Clone();
                mesh.Offset(visible.Position);
                float dis;
                Vector2 normal;
                Vector2 point;
                if (RayPolygonIntersect(pos, view, mesh.Points, out dis, out point, out normal))
                {
                    if (dis < hit.Distance)
                    {
                        hit.Visible = visible;
                        hit.HitPoint = point;
                        hit.Distance = dis;
                    }
                }
            }


            return hit;
        }



        private float _GetOffsetDirection(float angle)
        {
            var x = Math.PI * _ScanCycle / _DecisionTime;
            var y = (float)Math.Sin(x);
            return angle * y - (angle / 2);
        }

        protected override void _Update(float delta)
        {
            _ScanCycle += delta;
            _ScanCycle %= _DecisionTime;

        }


        protected override void _Shutdown()
        {
            _Unbind();
        }


        public static bool RayPolygonIntersect(Vector2 ro, Vector2 rd, Vector2[] polygon, out float t, out Vector2 point, out Vector2 normal)
        {
            t = float.MaxValue;
            point = ro;
            normal = rd;

            // Comparison variables.
            float distance;
            int crossings = 0;

            for (int j = polygon.Length - 1, i = 0; i < polygon.Length; j = i, i++)
            {
                if (RayLineIntersect(ro, rd, polygon[j], polygon[i], out distance))
                {
                    crossings++;

                    // If new intersection is closer, update variables.
                    if (distance < t)
                    {
                        t = distance;
                        point = ro + (rd * t);

                        Vector2 edge = polygon[j] - polygon[i];
                        normal = new Vector2(-edge.Y, edge.X).GetNormalized();
                    }
                }
            }

            return crossings > 0 && crossings % 2 == 0;
        }
        public static bool RayLineIntersect(Vector2 ro, Vector2 rd, Vector2 l1, Vector2 l2, out float t)
        {
            Vector2 seg = l2 - l1;
            Vector2 segPerp = new Vector2(seg.Y, -seg.X);


            float perpDotd = rd.DotProduct(segPerp);

            // If lines are parallel, return false.
            if (Math.Abs(perpDotd) <= float.Epsilon)
            {
                t = float.MaxValue;
                return false;
            }

            Vector2 d = l1 - ro;


            t = segPerp.DotProduct(d) / perpDotd;
            float s = new Vector2(rd.Y, -rd.X).DotProduct(d) / perpDotd;

            // If intersect is in right direction and in segment bounds, return true.
            return t >= 0.0f && s >= 0.0f && s <= 1.0f;
        }


        public bool IsWall(ENTITY entity_type)
        {
            return entity_type == ENTITY.WALL_EAST ||
                   entity_type == ENTITY.WALL_EAST_AISLE ||
                   entity_type == ENTITY.WALL_WESTERN ||
                   entity_type == ENTITY.WALL_WESTERN_AISLE ||
                   entity_type == ENTITY.WALL_NORTH ||
                   entity_type == ENTITY.WALL_NORTH_AISLE ||
                   entity_type == ENTITY.WALL_SOUTH ||
                   entity_type == ENTITY.WALL_SOUTH_AISLE;

        }

        public Hit Detect(float f, float view)
        {
            return _Detect(f , view);
        }

        public void MoveLeft()
        {
            if(_MoveController != null)
                _MoveController.TrunLeft();
        }

        public void MoveRight()
        {
            if (_MoveController != null)
                _MoveController.TrunRight();
        }

        public void StopTrun()
        {
            if (_MoveController != null)
                _MoveController.StopTrun();
        }
    }

    internal struct Exit
    {
        public float Direction { get; set; }

        public float Distance
        { get; set; }
    }


}