using Regulus.Project.ItIsNotAGame1.Game.Data;

namespace Regulus.Project.ItIsNotAGame1.Data
{
    public struct MazeUnitInfomation
    {
        public LEVEL_UNIT Type;

        public string Name;
    }
    public struct MazeInfomation
    {
        public int Dimension;
        public int Width;
        public int Height;

        public MazeUnitInfomation[] MazeUnits;
    }
    public class RealmInfomation
    {        
        public string Name;
        public MazeInfomation Maze;
        public TownInfomation Town;

        public bool HaveTown()
        {
            return string.IsNullOrEmpty(Town.Name) == false;
        }
    }

    public struct TownInfomation 
    {
        public string Name;
    }
}