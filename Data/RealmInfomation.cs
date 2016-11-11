namespace Regulus.Project.ItIsNotAGame1.Data
{

    public struct RealmInfomation
    {
        
        public string Name ;
        
        

        public int Dimension;

        public int Width;

        public int Height;

        public string Town;

        public bool IsMaze()
        {
            return Town == null;
        }
    }
}