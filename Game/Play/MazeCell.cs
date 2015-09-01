namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    
    internal struct MazeCell
    {
        public int Row;
        public int Column;

        
        public Regulus.CustomType.Flag<MAZEWALL> Walls;
    }
}