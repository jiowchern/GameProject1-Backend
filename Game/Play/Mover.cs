namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    public class Mover
    {
        private CustomType.Rect _Bounds;
        private float _Speed;

        public Mover(CustomType.Rect bounds, float speed)
        {
            // TODO: Complete member initialization
            this._Bounds = bounds;
            this._Speed = speed;
        }
        public IObservable GetOrbit(float second)
        {
            throw new System.NotImplementedException();
        }
    }
}