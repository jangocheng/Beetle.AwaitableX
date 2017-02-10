using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beetle.AwaitableX.Test
{
    class Program
    {
        static async void Main(string[] args)
        {


        }

        static async void Test()
        {
            System.Net.Sockets.Socket socket = null;
            SocketAwaiter socketwait = new SocketAwaiter(1024);
            var t = await socketwait.Receive(socket);
           
           
        }
    }
}
