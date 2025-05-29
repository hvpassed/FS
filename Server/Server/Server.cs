using System;
using System.Net.Sockets;
using NLog;
namespace FS.Server
{
    public class Server
    {
 
        private NetProxy net;
        
        private string ip = "127.0.0.1";

        private int port = 54321;
        public Server()
        {
            net = new NetProxy(ip, port);
        }
        private DateTime lastUpdateTimeStamp;
        private DateTime startUpTimeStamp;
        public void DoAwake()
        {
            startUpTimeStamp = lastUpdateTimeStamp = DateTime.Now;
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Info, SimpleServer.logger.Name, "Server DoAwake");
            SimpleServer.logger.Log(logEvent);
            net.DoAwake();
        }
        
        private const double updateInterval =0.033f-0.002f; //frame rate = 30
        private double deltaTime;
        private double timeSinceStartUp;
        public void DoStart()
        {
            net.DoStart();

        }

        public void Update()
        {
            var now = DateTime.Now;
            deltaTime = (now - lastUpdateTimeStamp).TotalSeconds;
            if (deltaTime > updateInterval) {
                lastUpdateTimeStamp = now;
                timeSinceStartUp = (now - startUpTimeStamp).TotalSeconds;
                net.DoUpdate();
            }

        }
        public void LateUpdate()
        {
            net.DoLateUpdate();
        }
        
    }
}