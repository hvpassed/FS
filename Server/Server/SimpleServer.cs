
using System;

using System.Threading;

using NLog;

namespace FS.Server
{
    public class SimpleServer
    {
        
        private static Server server;
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public static void Main()
        {
            OneThreadSynchronizationContext context = new OneThreadSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(context);
            server = new Server();
            server.DoAwake();
            server.DoStart();
            
            try
            {

                while (true)
                {

                    try {
                        
                        Thread.Sleep(2);
                        context.Update();
                        server.Update();
                        server.LateUpdate();
                    }
                    catch (ThreadAbortException e) {
                        return;
                    }
                    catch (Exception e) {
                        Console.WriteLine(e.ToString());
                    }
                                    
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}