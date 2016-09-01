using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace select_client
{
    class Program
    {
        static void Main(string[] args)
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            const int basePort = 11000;

            Console.Write("Input Client Number: ");
            int clientNo = (int)Convert.ToInt32(Console.ReadLine());

            IPEndPoint ep = new IPEndPoint(ipAddress, basePort + clientNo);

            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            client.Connect(ep);
            Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint);

            byte[] buffer = new byte[1024];

            while (true)
            {
                Console.Write("Input Message: ");
                string msg = Console.ReadLine();
                byte[] byteMsg = Encoding.ASCII.GetBytes(msg);
                int bytesSent = client.Send(byteMsg);
                Console.WriteLine("sent message");
                int byteRec = client.Receive(buffer);
                Console.WriteLine("Received from server: {0}", Encoding.ASCII.GetString(buffer, 0, byteRec));
                if(msg == "exit")
                {
                    break;
                }
            }

            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }
}
