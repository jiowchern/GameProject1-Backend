using Regulus.Project.ItIsNotAGame1.Data;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class MoveController : IMoveController
    {
        private readonly Entity _Player;
        

        public MoveController(Entity player)
        {
            _Player = player;
            
        }

        void IMoveController.Forward()
        {
            _Player.Move(0, false);
        }

        void IMoveController.Backward()
        {
            _Player.Move(180, false);
        }

        void IMoveController.StopMove()
        {
            _Player.Stop();
        }

        void IMoveController.TrunLeft()
        {
            _Player.Trun(-300);
        }

        void IMoveController.TrunRight()
        {
            _Player.Trun(300);
        }

        void IMoveController.StopTrun()
        {
            _Player.Trun(0);
        }

        void IMoveController.RunForward()
        {
            _Player.Move(0, true);
        }
    }
}