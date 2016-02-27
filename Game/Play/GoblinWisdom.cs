using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        private readonly List<IVisible> _Vision;


        private ActorMemory _ActorMemory;
        
        
        private IMoveController _MoveController;

        private float _ScanCycle;

        private readonly float _DecisionTime;
        
        

        private IEmotion _Emotion;

        private INormalSkill _NormalSkill;

        private ICastSkill _CastSkill;

        private readonly List<ACTOR_STATUS_TYPE> _UsableSkills;

        private IBattleSkill _BattleSkill;

        private bool _IsCollide;

        private bool _Investigate;

        public GoblinWisdom(Entity entiry)
        {
            _UsableSkills = new List<ACTOR_STATUS_TYPE>();
            _ActorMemory = new ActorMemory(entiry.GetVisible().EntityType);

            _DecisionTime = 0.5f;
                        
            _Vision = new List<IVisible>();
                        
            _Entiry = entiry;

            
            //asin(2 / sqrt(2 ^ 2 + 10 ^ 2))
        }
        

        protected override Regulus.BehaviourTree.ITicker _Launch()
        {
            _Entiry.InjuredEvent += _OnInjured;
            _Entiry.CollideTargets.JoinEvent += _HaveCollide;


            Guid actor = Guid.Empty ;
            Guid enemy = Guid.Empty ;

            var viewLength = (_Entiry as IVisible).View;
            _Bind();
            var builder = new Regulus.BehaviourTree.Builder();            
            var node = builder
                            .Selector()        
                                .Sequence()
                                    .Action((delta) => _DetectActor(viewLength, _GetOffsetDirection(120) , out actor))
                                    .Action((delta) => _NotSeen(actor))
                                    .Action((delta) => _Remember(actor))
                                    .Action((delta) => _Talk("!"))
                                .End()

                                .Sub(_CollisionWayfindingStrategy())
                                
                                .Sub(_AttackStrategy())

                                .Sub(_InvestigateStrategy())

                                .Sub(_DistanceWayfindingStrategy())

                                // 如果沒有敵人則切回一般狀態
                                .Sequence()
                                    .Action(_NotEnemy)                                    
                                    .Action(_InBattle)
                                    .Action(_ToNormal)                                    
                                .End()
                                

                                .Sub(_LootStrategy())
                                
                                // 沒事就前進
                                .Sequence()
                                    .Action(_MoveForward)
                                .End()
                            .End()
                        .Build();
            return node;
        }

        private ITicker _InvestigateStrategy()
        {
            var th = new TurnHandler(this);
            var builder = new Regulus.BehaviourTree.Builder();
            var node = builder
                    .Sequence()
                        .Action(_NotEnemy)
                        .Action(_NeedInvestigate )      
                                                
                        .Action(
                            (delta) =>
                            {
                                var angel = Regulus.Utility.Random.Instance.NextFloat(135, 225);
                                th.Input(angel);
                                return TICKRESULT.SUCCESS;
                            })
                        .Action((delta) =>
                        {
                            if (GetTrunSpeed() > 0)
                            {                                
                                return th.Run(delta);
                            }
                            return TICKRESULT.FAILURE;
                        })
                        .Action(_DoneInvestigate)
                    .End()
                .Build();
            return node;
        }

        private TICKRESULT _DoneInvestigate(float arg)
        {
            _Investigate = false;
            return TICKRESULT.SUCCESS;
        }

        private TICKRESULT _NeedInvestigate(float arg)
        {            
            if (_Investigate)
            {
                return TICKRESULT.SUCCESS;
            }

            return TICKRESULT.FAILURE;

        }

        private ITicker _LootStrategy()
        {
            float angle = 0.0f;
            var th = new TurnHandler(this);
            Guid target = Guid.Empty;
            var builder = new Regulus.BehaviourTree.Builder();
            var distance = 1f;
            var node = builder
                    .Sequence()
                        .Action((delta) => _FindLootTarget(out target) )
                        .Sequence()
                                .Action(
                                    (delta) =>
                                    {
                                        var result = _GetTargetAngle(target, ref angle);
                                        th.Input(angle);
                                        return result;
                                    })
                                .Action((delta) => th.Run(delta))

                                .Selector()
                                    .Not()
                                        .Sequence()
                                            .Action((delta) => _Not(_CheckDistance(target, distance)))
                                            .Action(_MoveForward)
                                        .End()
                                    .End()

                                    .Sequence()
                                        .Action((delta) => _CheckDistance(target, distance))
                                    .End()
                                .End()
                                .Action(_StopMove)
                                .Action((delta) => _GetTargetAngle(target, ref angle))
                                .Action((delta) => _CheckAngle(angle))
                            .End()
                        .Action((delta) => _Loot(target))
                    .End()
                .Build();
            return node;
        }

        private TICKRESULT _Loot(Guid target)
        {
            if (_NormalSkill != null)
            {
                _NormalSkill.Explore(target);
                _ActorMemory.Loot(target);
                return TICKRESULT.SUCCESS;
            }
            return TICKRESULT.FAILURE;
        }

        private TICKRESULT _FindLootTarget(out Guid target_id)
        {
            var target = _ActorMemory.FindLootTarget(_Vision);
            if (target != null)
            {
                target_id = target.Id;
                return TICKRESULT.SUCCESS;
            }
                
            target_id = Guid.Empty;
            return TICKRESULT.FAILURE;
        }

        private ITicker _CollisionWayfindingStrategy()
        {
            var th = new TurnHandler(this);
            
            var builder = new Regulus.BehaviourTree.Builder();
            return builder
                    .Sequence()
                        .Action(
                            (delta) =>
                            {
                                var result = _CheckCollide();
                                th.Input(Regulus.Utility.Random.Instance.NextFloat(1, 360));
                                return result;
                            } ) 
                        .Action((delta) => th.Run(delta))
                        .Action(_MoveForward)
                    .End()   
                .Build();
        }

        private TICKRESULT _CheckCollide()
        {
            if (_IsCollide)
            {
                _IsCollide = false;
                return TICKRESULT.SUCCESS;
            }                
            return TICKRESULT.FAILURE;
        }

        private ITicker _DistanceWayfindingStrategy()
        {
            var builder = new Regulus.BehaviourTree.Builder();
            var viewLength = _Entiry.GetVisible().View;

            var th = new TurnHandler(this);
            var od = new ObstacleDetector();
            od.OutputEvent += th.Input;
            return builder
                .Sequence()
                    // 是否碰到障礙物
                    .Action((delta) => _DetectObstacle(delta, viewLength / 2, _GetOffsetDirection(10.31f * 2)))

                    // 停止移動
                    //.Action(_StopMove)

                    // 檢查周遭障礙物
                    .Action((delta) => od.Detect(delta , _DecisionTime, _Entiry, this, viewLength, 180) )

                    // 旋轉至出口
                    .Action((delta) => th.Run(delta) )
                .End().Build();
        }
    

        private ITicker _ChangeToBattle()
        {
            var builder = new Regulus.BehaviourTree.Builder();
            return builder.Sequence()
                   .Action(_InNormal)
                   .Action(_ToBattle)
                   .End().Build();
        }

        private ITicker _AttackStrategy()
        {
            Guid enemy = Guid.Empty;
            
            var skill = ACTOR_STATUS_TYPE.NORMAL_IDLE;
            float distance = 0;
            float angle = 0.0f;
            var th = new TurnHandler(this);
            var builder = new Regulus.BehaviourTree.Builder();

            return builder
                .Sequence()
                    .Action((delta) => _FindEnemy(out enemy))

                    .Selector()
                        .Sub(_ChangeToBattle())
                        .Sequence()
                            .Action((delta) => _FindSkill(ref skill, ref distance))
                            .Sequence()
                                .Action(
                                    (delta) =>
                                    {
                                        var result = _GetTargetAngle(enemy, ref angle);
                                        th.Input(angle);
                                        return result;
                                    })
                                .Action((delta) => th.Run(delta))

                                .Selector()
                                    .Not()
                                        .Sequence()
                                            .Action((delta) => _Not(_CheckDistance(enemy, distance)))
                                            .Action(_MoveForward)
                                        .End()
                                    .End()

                                    .Sequence()
                                        .Action((delta) => _CheckDistance(enemy, distance))
                                    .End()
                                .End()
                                //.Action(_StopMove)
                                .Action((delta) => _GetTargetAngle(enemy, ref angle))
                                .Action((delta) => _CheckAngle(angle))
                            .End()
                            .Action((delta) => _UseSkill(skill))
                            .Action(() => new WaitSecondStrategy(0.1f) )
                        .End()

                    .End()

                    
                .End()
                .Build();

        }

        

       

        

        private TICKRESULT _Not(TICKRESULT check)
        {
            if(check == TICKRESULT.FAILURE)
                return TICKRESULT.SUCCESS;
            if (check == TICKRESULT.SUCCESS)
                return TICKRESULT.FAILURE;
            return TICKRESULT.RUNNING;            
        }

        private TICKRESULT _CheckAngle(float angle)
        {
            if (Math.Abs(angle ) < 45)
            {
                return TICKRESULT.SUCCESS;
            }
            return TICKRESULT.FAILURE;
        }

        private TICKRESULT _UseSkill(ACTOR_STATUS_TYPE skill)
        {
            if (_CastSkill != null)
            {
                _CastSkill.Cast(skill);
                return TICKRESULT.SUCCESS;
            }
            return TICKRESULT.FAILURE;
        }

        private TICKRESULT _CheckDistance(Guid enemy, float distance)
        {
            var target = _Find(enemy);

            if(target == null)
                return TICKRESULT.FAILURE;
            if(target.Position.DistanceTo(_Entiry.GetPosition()) <= distance )
                return TICKRESULT.SUCCESS;

            return TICKRESULT.FAILURE;
        }

        private TICKRESULT _GetTargetAngle(Guid id , ref float angle)
        {
            var target = _Find(id);
            if (target == null)
            {
                return TICKRESULT.FAILURE;
            }
            var position = _Entiry.GetPosition();
            var diff = target.Position - position;
            var a = Vector2.VectorToAngle(diff.GetNormalized());
            a += 360;
            a %= 360;

            angle = a - _Entiry.Direction;

#if UNITY_EDITOR
            var distance = target.Position.DistanceTo(position);
            var trunForce = Vector2.AngleToVector(a);
            var forcePos = position + trunForce * (distance);
            UnityEngine.Debug.DrawLine(new UnityEngine.Vector3(position.X, 0, position.Y), new UnityEngine.Vector3(forcePos.X, 0, forcePos.Y), UnityEngine.Color.green , _DecisionTime);
            //UnityEngine.Debug.Log("TurnDirection = " + _GoblinWisdom.TurnDirection);

#endif
            return TICKRESULT.SUCCESS;
        }

        private IVisible _Find(Guid id)
        {
            return (from v in _Vision where v.Id == id select v).FirstOrDefault();
        }

        private TICKRESULT _FindSkill(ref ACTOR_STATUS_TYPE skill, ref float distance)
        {
            var strength = _Entiry.GetStrength();
            if (strength < 0)
                return TICKRESULT.FAILURE;
            var len = _UsableSkills.Count;
            if(len == 0)
                return TICKRESULT.FAILURE;
            var index = Regulus.Utility.Random.Instance.NextInt(0, len);

            skill = _UsableSkills[index];

            distance = 1.3f;

            return TICKRESULT.SUCCESS;
        }

        private TICKRESULT _ToBattle(float arg)
        {
            if (_NormalSkill != null)
            {
                _NormalSkill.Battle();
                return TICKRESULT.SUCCESS;
            }

            return TICKRESULT.FAILURE;
        }

        private TICKRESULT _InNormal(float arg)
        {
            if(_NormalSkill != null)
                return TICKRESULT.SUCCESS;
            return TICKRESULT.FAILURE;            
        }

        private void _OnInjured(Guid target, float damage)
        {
            if (_ActorMemory.Hate(target, damage) == false)
            {
                _Investigate = true;
            }
        }

        private TICKRESULT _FindEnemy(out Guid enemy)
        {
            enemy = Guid.Empty;
            var result = _ActorMemory.FindEnemy(_Vision);
            if (result != null)
            {                
                enemy = result.Id;
                return TICKRESULT.SUCCESS;
            }
                
            return TICKRESULT.FAILURE;
        }

        private TICKRESULT _ToNormal(float arg)
        {
            if (_BattleSkill != null)
            {
                _BattleSkill.Disarm();
                return TICKRESULT.SUCCESS;
            }
                
            return TICKRESULT.FAILURE;
        }

        private TICKRESULT _InBattle(float arg)
        {
            if (_CastSkill != null)
            {
                return TICKRESULT.SUCCESS;
            }
            return TICKRESULT.FAILURE;
        }


        private TICKRESULT _NotEnemy(float arg)
        {
            var actor = _ActorMemory.FindEnemy(_Vision);
            if(actor == null)
                return TICKRESULT.SUCCESS;
            return TICKRESULT.FAILURE;
        }

        private TICKRESULT _Talk(string message)
        {
            if (_Emotion != null)
            {
                _Emotion.Talk(message);
                return TICKRESULT.SUCCESS;
            }
            return TICKRESULT.FAILURE;
        }

        private TICKRESULT _Remember(Guid id)
        {
            _ActorMemory.Add(id);
            
            return TICKRESULT.SUCCESS;
        }

        private TICKRESULT _NotSeen(Guid actor)
        {

            
            if (_ActorMemory.Have(actor))
            {
                return TICKRESULT.FAILURE;
                
            }
            return TICKRESULT.SUCCESS;
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
        private TICKRESULT _DetectActor(float distance, float angle, out Guid actor)
        {
            
            var target = _Detect(angle + _Entiry.Direction, distance);
            if (target.Visible != null && _IsActor(target.Visible.EntityType) )
            {
                actor = target.Visible.Id;
                return TICKRESULT.SUCCESS;
            }
            actor = Guid.Empty;
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

        

       
        private void _Bind()
        {
            Transponder.Query<IEmotion>().Supply += _SetEmotion;
            Transponder.Query<IEmotion>().Unsupply += _ClearEmotion;
            Transponder.Query<IVisible>().Supply += _SetIndividual;
            Transponder.Query<IVisible>().Unsupply += _ClearIndividual;
            Transponder.Query<IMoveController>().Supply += _SetMoveController;
            Transponder.Query<IMoveController>().Unsupply += _ClearMoveController;

            Transponder.Query<INormalSkill>().Supply += _SetNormalSkill;
            Transponder.Query<INormalSkill>().Unsupply += _ClearNormalSkill;

            Transponder.Query<ICastSkill>().Supply += _SetCastSkill;
            Transponder.Query<ICastSkill>().Unsupply += _ClearCastSkill;

            Transponder.Query<IBattleSkill>().Supply += _SetBattleSkill;
            Transponder.Query<IBattleSkill>().Unsupply += _ClearBattleSkill;

        }

        private void _Unbind()
        {
            Transponder.Query<IEmotion>().Supply -= _SetEmotion;
            Transponder.Query<IEmotion>().Unsupply -= _ClearEmotion;
            Transponder.Query<IMoveController>().Supply -= _SetMoveController;
            Transponder.Query<IMoveController>().Unsupply -= _ClearMoveController;
            Transponder.Query<IVisible>().Supply -= _SetIndividual;
            Transponder.Query<IVisible>().Unsupply -= _ClearIndividual;

            Transponder.Query<INormalSkill>().Supply -= _SetNormalSkill;
            Transponder.Query<INormalSkill>().Unsupply -= _ClearNormalSkill;

            Transponder.Query<ICastSkill>().Supply -= _SetCastSkill;
            Transponder.Query<ICastSkill>().Unsupply -= _ClearCastSkill;

            Transponder.Query<IBattleSkill>().Supply -= _SetBattleSkill;
            Transponder.Query<IBattleSkill>().Unsupply -= _ClearBattleSkill;
        }

        private void _ClearBattleSkill(IBattleSkill obj)
        {
            _BattleSkill = null;
        }

        private void _SetBattleSkill(IBattleSkill obj)
        {
            _BattleSkill = obj;
        }

        private void _ClearCastSkill(ICastSkill obj)
        {
            _CastSkill.HitNextsEvent -= _AddSkills;
            _CastSkill = null;

            _UsableSkills.Clear();
        }

        private void _SetCastSkill(ICastSkill obj)
        {
            _CastSkill = obj;
            _CastSkill.HitNextsEvent += _AddSkills;
            _UsableSkills.AddRange(_CastSkill.Skills);
        }

        private void _AddSkills(ACTOR_STATUS_TYPE[] obj)
        {
            _UsableSkills.AddRange(obj);
        }

        private void _ClearNormalSkill(INormalSkill obj)
        {
            _NormalSkill = null;
        }

        private void _SetNormalSkill(INormalSkill obj)
        {
            _NormalSkill = obj;
        }

        private void _ClearEmotion(IEmotion obj)
        {
            _Emotion = null;
        }
        private void _SetEmotion(IEmotion obj)
        {
            _Emotion = obj;
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
            //Unity Debug Code
#if UNITY_EDITOR
            UnityEngine.Debug.DrawRay(new UnityEngine.Vector3(pos.X, 0, pos.Y), new UnityEngine.Vector3(view.X, 0, view.Y), UnityEngine.Color.blue, 0.5f);
#endif

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

            _ActorMemory.Forget(delta);
        }


        protected override void _Shutdown()
        {
            _Entiry.CollideTargets.JoinEvent -= _HaveCollide;

            _Entiry.InjuredEvent -= _OnInjured;
            _Unbind();
        }

        private void _HaveCollide(IEnumerable<IIndividual> instances)
        {
            _IsCollide = true;
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

        private bool _IsActor(ENTITY entity_type)
        {
            return entity_type == ENTITY.ACTOR1 ||
                   entity_type == ENTITY.ACTOR2 ||
                   entity_type == ENTITY.ACTOR3 ||
                   entity_type == ENTITY.ACTOR4 ||
                   entity_type == ENTITY.ACTOR5;
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

        public float GetTrunSpeed()
        {
            if (_MoveController != null)
            {
                var status = _Entiry.GetStatus();
                var skill = Resource.Instance.FindSkill(status);
                var caster = new SkillCaster(skill, new Determination(skill));

                return caster.GetTurnRight();
            }

            return 0;
        }
    }

    internal struct Exit
    {
        public float Direction { get; set; }

        public float Distance
        { get; set; }
        
    }


}