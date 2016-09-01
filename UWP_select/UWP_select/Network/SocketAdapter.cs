using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UWP_select.ViewModels;

namespace UWP_select.Network
{
    class SocketAdapter
    {
        public SocketAdapter(int port)
        {
            Port = port;
        }

        private bool _continue = false;
        private object _thisLock = new object();

        public int Port { get; private set; }
        public bool IsWorking { get { return _continue; } }
        

        public async void Begin(MainPageViewModel mainPageViewModel)
        {
            lock (_thisLock)
            {
                if (_continue)
                {
                    return;
                }
                _continue = true;
            }

            /*int count = 1;
            while (_continue)
            {   
                mainPageViewModel.TbxMsg += count + ", ";
                count++;
                await Task.Delay(1000);
            }*/
            byte[] bytes = new byte[1024];

            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                // This example uses port 11000 on the local computer.
                //IPHostEntry ipHostInfo = Dns.Resolve();
                //IPHostEntry ipHostInfo = new DnsEndPoint(Dns.GetHostName(), Port);
                /*IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("localhost");
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Port);*/

                //IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                //IPHostEntry ipHostInfo = await Dns.GetHostAddressesAsync(Dns.GetHostName());
                //IPAddress ipAddress = ipHostInfo.AddressList[0];
                //IPAddress[] ipAddresses = await Dns.GetHostAddressesAsync(Dns.GetHostName());
                IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(Dns.GetHostName());
                IPAddress ipAddress = null;
                for (int i = 0; i < ipHostInfo.AddressList.Length; i++)
                {
                    if (AddressFamily.InterNetwork == ipHostInfo.AddressList[i].AddressFamily)
                    {
                        ipAddress = ipHostInfo.AddressList[i];
                    }
                }
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Port);

                // Create a TCP/IP  socket.
                Socket sender = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    int count = 1;
                    sender.Connect(remoteEP);

                    mainPageViewModel.TbxMsg += "Socket connected to {0}" +
                        sender.RemoteEndPoint.ToString();

                    while (_continue)
                    {
                        // Encode the data string into a byte array.
                        byte[] msg = Encoding.ASCII.GetBytes("Msg is [" + count + "].");

                        // Send the data through the socket.
                        int bytesSent = sender.Send(msg);

                        // Receive the response from the remote device.
                        int bytesRec = sender.Receive(bytes);
                        //Console.WriteLine("Echoed test = {0}",
                        //    Encoding.ASCII.GetString(bytes, 0, bytesRec));
                        mainPageViewModel.TbxMsg += "Echoed test = " +
                            Encoding.ASCII.GetString(bytes, 0, bytesRec) + "\r\n";

                        count++;
                        await Task.Delay(1000);
                    }
                    
                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Dispose();
                }
                catch (ArgumentNullException ane)
                {
                    mainPageViewModel.TbxMsg += "ArgumentNullException : " + ane.ToString();
                }
                catch (SocketException se)
                {
                    mainPageViewModel.TbxMsg += "SocketException : " + se.ToString();
                }
                catch (Exception e)
                {
                    mainPageViewModel.TbxMsg += "Unexpected exception : " + e.ToString();
                }
            }
            catch (Exception e)
            {
                mainPageViewModel.TbxMsg += e.ToString();
            }
            finally
            {
                _continue = false;
            }
        }

        public void Stop()
        {
            lock (_thisLock)
            {
                _continue = false;
            }
        }
    }
}
