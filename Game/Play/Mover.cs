namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Mover
    {
        private Regulus.CustomType.Polygon _Body;
        private float _Speed;

        public Mover(Regulus.CustomType.Polygon body, float speed)
        {                        
            this._Body =  body;
            _Speed = speed;
        }
        public IObservable GetOrbit(float angle , float second)
        {
            throw new System.NotImplementedException();
        }
    }
}