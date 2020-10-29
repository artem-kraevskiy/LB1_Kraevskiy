using System;
using System.Threading;

namespace Server
{
    class Program
    {
        static ServerClass server;
        static Thread listenThread;
        static void Main(string[] args)
        {
            try
            {
                server = new ServerClass();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start();
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
