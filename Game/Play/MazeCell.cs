namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    
    internal struct MazeCell
    {
        public int Row;
        public int Column;

        
        public CustomType.Flag<MAZEWALL> Walls;
    }
}