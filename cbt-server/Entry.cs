using System;
using System.Net;
using NElniorPackS;
using System.Threading;
using System.Reflection;
using cbt_server.HttpIO;
using System.Net.Sockets;

namespace cbt_server
{
    internal class AcmySureServer
    {
        private Socket sock;
        public static string baseDir = "";
        // I need to prottect this:
        public static ServerInfo serverInfo;
        public AcmySureServer (string address, int port)
        {
            this.sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(address), port);
            this.sock.Bind(endPoint);
            // Initializing ServerInfo
            serverInfo = new ServerInfo();
            serverInfo.connections = new Socket[0];
            serverInfo.connectionCount = 0;
            string dir = Assembly.GetEntryAssembly().Location;
            // Just windows OS
            int lastIndex = dir.LastIndexOf("\\");
            baseDir = dir.Substring(0, lastIndex);
        }
        public void manageConnection (object remoteConnection)
        {
            Socket readyConnection = (Socket)remoteConnection;
            // When I need to receive asynchronous data.
            // readyConnection.ReceiveTimeout = 38000;
            // readyConnection.SendTimeout = 100;
            // (1Kb + 4b)
            byte[] data = new byte[1026];
            int received;
            readyConnection.ReceiveBufferSize = data.Length;
            try
            {
                // 1st process:
                received = readyConnection.Receive(data);
                
                Anully anully = new Anully(data, received);
                if (anully)
                {
                    Answer answer = new Answer(anully);
                    answer.dispatch(readyConnection);
                }
                else
                {
                    readyConnection.Close();
                    readyConnection.Dispose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("0x345: {0}", e.Message);
                
                readyConnection.Close();
                readyConnection.Dispose();
            }
        }
        public void start (int count)
        {
            bool status = false;
            try
            {
                this.sock.Listen(count);
                Console.WriteLine("The {0} server it's running..", this.sock.LocalEndPoint);
                status = true;
            }
            catch (Exception e)
            {
                status = false;
                Console.WriteLine("0x445: {0}", e.Message);
            }
            finally
            {
                if (status) 
                {
                    SocketAsyncEventArgs socketArg = new SocketAsyncEventArgs();
                    socketArg.Completed += new EventHandler<SocketAsyncEventArgs>(this.OnAcceptedSocket);
                    // Start to get all connections
                    bool willRaceEvent = this.sock.AcceptAsync(socketArg);
                    if (!willRaceEvent)
                        this.OnAcceptedSocket((object)this, socketArg);
                }
            }
        }
        public void OnAcceptedSocket (object sender, SocketAsyncEventArgs socketAsyncArg)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(this.manageConnection));
            thread.IsBackground = true;
            thread.Start(socketAsyncArg.AcceptSocket);
            // cleaning..
            socketAsyncArg.Dispose();
            // To be continue there..
            socketAsyncArg = new SocketAsyncEventArgs();
            socketAsyncArg.Completed += new EventHandler<SocketAsyncEventArgs>(this.OnAcceptedSocket);

            bool willRaceEvent = this.sock.AcceptAsync(socketAsyncArg);
            if (!willRaceEvent)
                this.OnAcceptedSocket(sender, socketAsyncArg);
        }
        internal static void Main (string[] Args)
        {
            AcmySureServer acmy = new AcmySureServer("127.0.0.45", 80);
            acmy.start(71);
            // local Hook:
            while (true)
                Console.Read();
        }
    }
}