using System;
using System.Linq;

namespace Regulus.Project.ItIsNotAGame1.Game.Play
{
    internal class ResourceBag : Bag
    {
        

        public ResourceBag()
        {
            _Supplement();
            RemoveEvent += _Supplement;
        }

        private void _Supplement(Guid id)
        {
            _Supplement();
        }

        private void _Supplement()
        {
            if ( this.Count() <= 1  )
            {
                var itemProvider = new ItemProvider();
                Add(itemProvider.FromStolen());
            }
        }
    }
}