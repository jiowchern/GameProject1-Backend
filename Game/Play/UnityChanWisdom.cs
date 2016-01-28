using System;
using System.Linq;
using System.Text;

using Regulus.Framework;
using Regulus.Remoting;
using Regulus.Utility;
using FluentBehaviourTree;

using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{

    public class UnityChanWisdom : Wisdom
    {
        private readonly Entity _Entity;
        

        private IVisible _Visible;

        private IMoveController _MoveController;

        private IBattleSkill _BattleSkill;

        private bool _Hurt;

        private IEmotion _Emotion;

        public UnityChanWisdom(Entity entity) 
        {            
            _Entity = entity;
            _Visible = entity;             
                        
        }

        private float _RabbitCatHappyBirthdayTime;

        private BehaviourTreeStatus _RabbitCatHappyBirthday(TimeData arg)
        {
            _RabbitCatHappyBirthdayTime -= arg.deltaTime;
            if (_RabbitCatHappyBirthdayTime <= 0 && _Emotion != null)
            {
                var messags = new[]
                {
                    "\u5154\u8c93\u751f\u65e5\u5feb\u6a02^O^~",
                    "\u5154\u8c93\u751f\u65e5\u5feb\u6a02XD~"
                };
                var index = Regulus.Utility.Random.Instance.NextInt(0, 2);
                var msg  = messags[index];
                _Emotion.Talk(msg);                
                _RabbitCatHappyBirthdayTime = Regulus.Utility.Random.Instance.NextFloat(5, 10);
                
            }
            return BehaviourTreeStatus.Success;
        }

        public static string Utf16ToUtf8(string utf16String)
        {
            // Get UTF16 bytes and convert UTF16 bytes to UTF8 bytes            
            byte[] utf16Bytes = Encoding.Default.GetBytes(utf16String);
            byte[] utf8Bytes = Encoding.Convert(Encoding.Default, Encoding.UTF8, utf16Bytes);

            // Return UTF8 bytes as ANSI string
            return Encoding.UTF8.GetString(utf8Bytes);
        }

        private float _TrunTime;
        private BehaviourTreeStatus _RandomTrun(TimeData arg)
        {
            _TrunTime -= arg.deltaTime;
            if (_TrunTime <= 0 && _MoveController != null)
            {
                var index = Regulus.Utility.Random.Instance.NextInt(0, 4);
                if (index == 0 || index == 1)
                {
                    _MoveController.StopTrun();
                    _TrunTime = Regulus.Utility.Random.Instance.NextFloat(1, 10);
                }
                if (index == 2)
                {
                    _MoveController.TrunLeft();
                    _TrunTime = Regulus.Utility.Random.Instance.NextFloat(1, 3);
                }                    
                if (index == 3)
                {
                    _MoveController.TrunRight();
                    _TrunTime = Regulus.Utility.Random.Instance.NextFloat(1, 3);
                }

                
            }
            return BehaviourTreeStatus.Success;
        }

        

        private BehaviourTreeStatus _Stop(TimeData arg)
        {
            _Entity.Stop();
            return BehaviourTreeStatus.Success;
        }

        private double _Random()
        {
            return Regulus.Utility.Random.Instance.NextFloat(0.0f, 1.0f);
        }

        private BehaviourTreeStatus _Talk(TimeData arg)
        {
            string[] messages = new string[]{ "Hurts", "QAQ", "Do not hurt me " };
            var index = Regulus.Utility.Random.Instance.NextInt(0,3);
           if (_Emotion != null)
                _Emotion.Talk(messages[index]);
           
           return BehaviourTreeStatus.Success;
        }

        private BehaviourTreeStatus _Disarm(TimeData arg1)
        {
            if(_BattleSkill != null)
                _BattleSkill.Disarm();

            return BehaviourTreeStatus.Success;
        }

        private bool _IsInBattle()
        {
            return _BattleSkill != null;
        }

        bool _CheckCollison()
        {
            var res = _Entity.GetCollisionTargets().Any();
            return res;
        }

        private BehaviourTreeStatus _Run(TimeData arg)
        {
            if (_MoveController != null)
                _MoveController.RunForward();
            return BehaviourTreeStatus.Success;
        }
        private BehaviourTreeStatus _Move(TimeData arg)
        {
            if (_MoveController != null)
                _MoveController.Forward();
            return BehaviourTreeStatus.Success;
        }

        private BehaviourTreeStatus _TurnToBack(TimeData arg)
        {
            _Entity.SetDirection(Regulus.Utility.Random.Instance.NextFloat(90, 270));
            return BehaviourTreeStatus.Success;
        }

        
        protected override IBehaviourTreeNode _Launch()
        {

            var transponder = Transponder;

            transponder.Query<IEmotion>().Unsupply += _ClearEmotion;
            transponder.Query<IEmotion>().Supply += _GetEmotion;
            transponder.Query<IMoveController>().Unsupply += _ClearMoveController;
            transponder.Query<IMoveController>().Supply += _GetMoveController;
            transponder.Query<IBattleSkill>().Supply += _GetBattle;
            transponder.Query<IBattleSkill>().Unsupply += _ClearBattle;


            _Visible.StatusEvent += _GetStatus;

            var builder = new BehaviourTreeBuilder();

            return builder.Selector("root")
                        .Sequence("Idle Walk")
                            .Condition("speed == 0", (td) => _Entity.Speed == 0.0f)
                                .Sequence("")
                                    .Condition("speed == 0", (td) => _Random() > 0.5f)
                                        .Do("Run", _Run)
                                    .Condition("speed == 0", (td) => _Random() > 0.5f)
                                        .Do("Move", _Move)
                                    .Condition("speed == 0", (td) => _Random() > 0.5f)
                                        .Do("", _Disarm)
                                .End()
                        .End()

                        .Sequence(" ab c ")
                            .Do("RandomTrun", _RandomTrun)
                            .Condition("is hurt ", (td) => _Hurt)
                                    .Sequence("Talk")
                                        .Do("", _Talk)
                                        .Do("clear Hurt",
                                            (td) =>
                                            {
                                                _Hurt = false;
                                                return BehaviourTreeStatus.Success;
                                            })
                                    .End()
                        .End()

                        .Sequence("Battle To Normal")
                            .Condition("if in battle", (td) => _IsInBattle())
                                .Do("Run", _Run)
                        .End()
                    .End().Build();


        }

        protected override void _Update(float delta)
        {
            
        }

        protected override void _Shutdown()
        {
            var transponder = Transponder;
            transponder.Query<IEmotion>().Unsupply -= _ClearEmotion;
            transponder.Query<IEmotion>().Supply -= _GetEmotion;

            transponder.Query<IMoveController>().Unsupply -= _ClearMoveController;
            transponder.Query<IMoveController>().Supply -= _GetMoveController;

            transponder.Query<IBattleSkill>().Supply -= _GetBattle;
            transponder.Query<IBattleSkill>().Unsupply -= _ClearBattle;
        }

        

        private void _ClearEmotion(IEmotion obj)
        {
            _Emotion = null;
        }

        private void _GetEmotion(IEmotion obj)
        {
            _Emotion = obj;
        }

        private void _GetStatus(VisibleStatus obj)
        {
            _Hurt = obj.Status == ACTOR_STATUS_TYPE.KNOCKOUT1 || obj.Status == ACTOR_STATUS_TYPE.DAMAGE1;
        }

        private void _ClearBattle(IBattleSkill obj)
        {
            _BattleSkill = null;
        }

        private void _GetBattle(IBattleSkill obj)
        {
            _BattleSkill = obj;
        }

        private void _ClearMoveController(IMoveController obj)
        {
            _MoveController = null;
        }

        private void _GetMoveController(IMoveController obj)
        {
            _MoveController = obj;
        }

        
    }
}