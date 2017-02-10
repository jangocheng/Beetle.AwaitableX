using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Beetle.AwaitableX.Test
{
    class Program
    {
        static void Main(string[] args)
        {

            Test();
            Console.Read();
        }

        static async void Test()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("127.0.0.1", 9090);
            byte[] data = Encoding.UTF8.GetBytes("socket awit test \r\n");
            SocketAwaiter socketwait = new SocketAwaiter(1024);
            socketwait.SetBuffer(data, 0, data.Length);
            var s = await socketwait.Send(socket);
            if (s.Target.SocketError == SocketError.Success)
            {
                var r = await socketwait.Receive(socket);
                var result = r.Target.Result;
                var line = Encoding.UTF8.GetString(result.Array, 0, result.Count);
                Console.Write(line);
            }
            

        }
    }
}
