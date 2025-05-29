using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using FS.Network;
using NLog;

namespace FS.Server
{
    public class NetBase:IDisposable
    {
        public bool IsDisposed = false;
        
        
        
        public virtual void Dispose()
        {
            IsDisposed = true;
            // Dispose logic here
        }

 
    }

    public class NetProxy : NetBase
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private TcpListener tcpListener;
        private IPAddress ipAddress;
        private int port;
        
        private int clientId = 0;

        private Room room;
        
        
        public NetProxy(string ip, int port)
        {
            ipAddress = System.Net.IPAddress.Parse(ip);

            this.port = port;
        }

        public int getId()
        {
            return Interlocked.Increment(ref clientId);
        }
        public void DoAwake()
        {
            room = new Room();
            
            tcpListener = new TcpListener(ipAddress, port);
            tcpListener.Start();
        }

        public void DoStart()
        {
            AcceptSync();
            logger.Info("Waiting for client connection...");
        }

        public void DoUpdate()
        {
            room.DoUpdate();
        }
        public void DoLateUpdate()
        {
            room.DoLateUpdate();
        }
        
        public async Task AcceptSync()
        {


            while (!IsDisposed)
            {
                try
                {            
                    
                    await AcceptClient();


                }
                catch (Exception ex)
                {

                    logger.Error($"Error accepting client: {ex.Message}");
                }
            }

            return;
        }

        public async Task<ClientSession> AcceptClient()
        {
            TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
            ClientSession clientSession = new ClientSession(tcpClient);
            logger.Info($"Accepting client: {tcpClient.Client.RemoteEndPoint}");
            room.AddPlayer(clientSession,getId());

            return clientSession;
        }


    }
}