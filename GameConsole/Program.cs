using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Regulus.Project.ItIsNotAGame1;
using Regulus.Project.ItIsNotAGame1.Game;
using Regulus.Remoting;
using Regulus.Utility;
using Regulus.Utility.WindowConsoleAppliction;

namespace GameConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            



            var console = new ClientConsole();


            console.Run();




        }
    }

    internal class ClientConsole : WindowConsole
    {
        private Regulus.Utility.Updater _Updater;
        public ClientConsole()
        {
            _Updater = new Updater();
        }
        protected override void _Launch()
        {
            var builder = new Regulus.Protocol.AssemblyBuilder();

            var dataAsm = Assembly.LoadFile(System.IO.Path.GetFullPath("ItIsNotAGame1Data.dll"));

            var asm = builder.BuildMemory(dataAsm, "Regulus.Project.ItIsNotAGame1.Protocol");

            var protocl = asm.CreateInstance("Regulus.Project.ItIsNotAGame1.Protocol") as IProtocol;
            var client = new Regulus.Framework.Client<IUser>(this.Viewer, this.Command);
            throw new NotImplementedException();
            client.Selector.AddFactoty("online", new RemotingUserFactory(protocl,null));

            _Updater.Add(client);
        }

        protected override void _Update()
        {
            _Updater.Working();
        }

        protected override void _Shutdown()
        {
            _Updater.Shutdown();
        }
    }
}
