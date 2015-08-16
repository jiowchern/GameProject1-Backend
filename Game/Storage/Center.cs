using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Framework;
using Regulus.Game;
using Regulus.Project.ItIsNotAGame1.Data;
using Regulus.Remoting;
using Regulus.Utility;

namespace Regulus.Project.ItIsNotAGame1.Game.Storage
{
    public class Center : ICore
    {
        private readonly Hall _Hall;

        private readonly IStorage _Stroage;

        private readonly Updater _Update;

        public Center(IStorage storage)
        {
            _Stroage = storage;
            _Hall = new Hall();
            _Update = new Updater();
        }

        void ICore.AssignBinder(ISoulBinder binder)
        {
            _Hall.PushUser(new User(binder, _Stroage));
        }

        bool IUpdatable.Update()
        {
            _Update.Working();
            return true;
        }

        void IBootable.Launch()
        {
            _Update.Add(_Hall);
        }

        void IBootable.Shutdown()
        {
            _Update.Shutdown();
        }
    }
}
