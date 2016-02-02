using System;
using System.Collections.Generic;
using System.Linq;

using Regulus.BehaviourTree;



using Vector2 = Regulus.CustomType.Vector2;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class ObstacleDetector : IAction
    {
        private readonly float _DecisionTime;

        private float _TimeCounter;

        private readonly Entity _Entiry;

        private readonly GoblinWisdom _GoblinWisdom;

        private readonly float _Distance;

        private readonly float _ScanAngle;

        private readonly bool _MaxAngle;

        private List<Exit> _Nears;

        private int _Done;
        public ObstacleDetector(float decision_time, Entity entiry, GoblinWisdom goblin_wisdom , float distance , float scan_angle , bool max_angle)
        {
            _Nears = new List<Exit>();
            _DecisionTime = decision_time;
            _Entiry = entiry;
            _GoblinWisdom = goblin_wisdom;
            _Distance = distance;
            _ScanAngle = scan_angle;
            _MaxAngle = max_angle;
        }

        TICKRESULT ITicker.Tick(float delta)
        {


            
                
            
            var pos = _Entiry.GetPosition();
            _TimeCounter += delta;
            if (_TimeCounter < _DecisionTime)
            {
                var view = (float)_Distance ;
                var x = Math.PI * _TimeCounter / _DecisionTime;
                var y = (float)Math.Sin(x);
                var a = _ScanAngle * y - (_ScanAngle / 2);
                

                var target = _GoblinWisdom.Detect(a + _Entiry.Direction, view);

                if (target.Visible == null || (target.Visible != null && _GoblinWisdom.IsWall(target.Visible.EntityType)))
                {


                    var distance = target.HitPoint.DistanceTo(pos) ;
                    distance =(float) Math.Floor(distance);
                    var vector = target.HitPoint - pos;
                    var hitAngle = Vector2.VectorToAngle(vector.GetNormalized());
                    hitAngle += 360;
                    hitAngle %= 360;

                    //_UnityDrawLine(hitAngle, pos , Color.red);

                    _Nears.Add(new Exit() { Distance = distance, Direction = hitAngle });

                }


                return TICKRESULT.RUNNING;
            }


            var sortedDirections = (_MaxAngle)?
                                    from e in _Nears
                                   let diff = Math.Abs(e.Direction - _Entiry.Direction)
                                   where diff > 0.0f
                                   orderby diff descending
                                    select e 
                                   :
                                   from e in _Nears
                                   let diff = Math.Abs(e.Direction - _Entiry.Direction)
                                   where diff > 0.0f
                                   orderby diff 
                                   select e ;
            var soteds = from e in sortedDirections orderby e.Distance descending select e;
            var first = soteds.FirstOrDefault();            

            _GoblinWisdom.TurnDirection = first.Direction - _Entiry.Direction;



            //var pos = _Entiry.GetPosition();            
            /*var trunForce = Vector2.AngleToVector(first.Direction);
            var forcePos = pos + trunForce * (_Distance );
            UnityEngine.Debug.DrawLine(new UnityEngine.Vector3(pos.X, 0, pos.Y), new UnityEngine.Vector3(forcePos.X, 0, forcePos.Y), UnityEngine.Color.yellow, _DecisionTime);
            _Done ++;
            UnityEngine.Debug.Log("ObstacleDetector Tick " + _Done );*/
            return TICKRESULT.SUCCESS;
        }


        /*private void _UnityDrawLine(float hitAngle, Vector2 pos , UnityEngine.Color color)
        {
            var trunForce = Vector2.AngleToVector(hitAngle);
            var forcePos = pos + trunForce * (_Distance );
            UnityEngine.Debug.DrawLine(
                new UnityEngine.Vector3(pos.X, 0, pos.Y),
                new UnityEngine.Vector3(forcePos.X, 0, forcePos.Y),
                color,
                _DecisionTime);
        }*/

        void IAction.Start()
        {
            _Done = 0;
            _Nears.Clear();
            _TimeCounter = 0.0f;
            
        }

        void IAction.End()
        {
            
        }
    }
}