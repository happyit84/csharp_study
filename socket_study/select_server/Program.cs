using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace select_server
{
    // State object for reading client data asynchronously
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();

        public Socket[] sockets = null;
    }

    class Program
    {
        public Program() { }

        public static readonly int SOCKET_NUM = 3;

        public static ManualResetEvent exitEvent = new ManualResetEvent(false);

        public Socket[] Sockets { get; set; } = new Socket[SOCKET_NUM];

        public static void StartListening(Socket [] sockets)
        {
            // Establish the local endpoint for the socket.
            // The DNS name of the computer
            // running the listener is "host.contoso.com".
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            int basePort = 11000;
            IPEndPoint[] endPoints = {
                new IPEndPoint(ipAddress, basePort + 0),
                new IPEndPoint(ipAddress, basePort + 1),
                new IPEndPoint(ipAddress, basePort + 2)
            };

            // Create a TCP/IP socket.
            const int BACK_LOG = 100;
            for (int i = 0; i < sockets.Length; i++)
            {
                sockets[i] = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sockets[i].Bind(endPoints[i]);
                sockets[i].Listen(BACK_LOG);

                // Start an asynchronous socket to listen for connections.
                Console.WriteLine("Waiting for a connection...{0}:{1}", endPoints[i].Address, endPoints[i].Port);
                //StateObject state = new StateObject();
                //state.workSocket = sockets[i];
                //state.sockets = sockets;
                sockets[i].BeginAccept(new AsyncCallback(AcceptCallback), sockets[i]);
            }

            exitEvent.WaitOne();

            for(int i=0; i < sockets.Length; i++)
            {
                sockets[i].Shutdown(SocketShutdown.Both);
                sockets[i].Close();
            }
        }

        public static void Main()
        {
            Program program = new select_server.Program();
            StartListening(program.Sockets);
            Console.WriteLine("Exit");
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;

            Console.WriteLine("{0} Accepted!", handler.RemoteEndPoint);

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, 
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString();
                
                Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);

                handler.Send(state.buffer);

                if (content == "exit")
                {                    
                    exitEvent.Set();
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }
    }
}
