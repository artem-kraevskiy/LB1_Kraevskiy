using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Server
{
    class ClientClass
    {
        public string Id { get; private set; }
        public NetworkStream Stream { get; private set; }
        string Name;
        TcpClient client;
        ServerClass server;

        public ClientClass(TcpClient tcpClient, ServerClass server1)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = server1;
            server1.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                string message = GetMessage();
                Name = message;

                message = Name + " подключился";
                server.BroadcastMessage(message, this.Id);
                Console.WriteLine(message);
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format(Name + ": " + message);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                    }
                    catch
                    {
                        message = String.Format(Name + ": покинул чат");
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        public void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
