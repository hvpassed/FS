using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FS.Utils;
using FS.Logic;
using FS.Network;
using FS.Utils;
using NLog;

namespace FS.Server
{
    public class ClientSession:NetBase,ISession,IMessageDispatcher
    {
        private Room _belongToRoom;
        
        private static Logger logger  = LogManager.GetCurrentClassLogger();
        private Receiver _receiver;
        private Sender _sender;

        private TcpClient _tcpClient;
        private bool isConnected = false;
        
        public event Action<ClientSession> onDisconnect;
        private CancellationTokenSource _cancellationTokenSource;

        public Action<ClientSession> OnDisconnectHandle;
        
        public long recvivedPacket = 0,
            sendedPacket = 0;
        
        public ClientSession(TcpClient tcpClient)
        {
            
            _tcpClient = tcpClient;
            isConnected = true;
        }

        public EndPoint GetRemoteAddress()
        {
            return _tcpClient.Client.RemoteEndPoint;
        }
        
        public void DoAwake( )
        {
            _receiver = new Receiver(_tcpClient.GetStream());
            _sender = new Sender(_tcpClient.GetStream());
            _cancellationTokenSource = new CancellationTokenSource();
            
        }


        public void DoStart(Room belongto)
        {

            _belongToRoom = belongto;
            // MsgCtl msgCtl = new MsgCtl();
            // msgCtl.CtlType = NetworkCtlType.SYN;
            // var bytes = MessageFactory.CreateMessage(msgCtl);
            // //logger.Info($"send length {bytes.Length}");
            // //HexFileHelper.SaveBytesAsHexString(bytes, "send.txt");
            // Send(bytes);

            StartReceiving();

        }

        public async Task StartReceiving()
        {
            logger.Info($"start recving");
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    byte[] data = await _receiver.Recv();
                    var message = MessageFactory.ParseMessage(data, out var type, false);
                    //logger.Info($"Recvied message {data.Length} {type} {data}");
                    recvivedPacket++;
                    Dispatcher(this, message);
                }
            }
            catch (IOException ex)
            {
                logger.Info("Disconnect");
                Disconnect();
                Dispose();
            }
            catch (Exception ex)
            {
                logger.Error($"{ex.StackTrace}");
                logger.Error($"Receiving loop stopped: {ex.Message}");
            }

            Disconnect();
        }

        public async Task Send(byte[] data)
        {
            sendedPacket++;
            await _sender.Send(data);
        }


        public async Task Send(ISendable data)
        {
            if (data == null)
            {
                return;
            }

            sendedPacket++;
 
            //var message = ;
 
            await _sender.Send(MessageFactory.CreateMessage(data));

        }
            
        public bool CheckConnected()
        {
            //return _tcpClient.Client.Poll(0, SelectMode.SelectRead) && _tcpClient.Client.Available == 0;
            return _tcpClient.Connected;
        }
        
        public void Disconnect()
        {
            _tcpClient.Close();

        }
        public override void Dispose()
        {
            if (_tcpClient != null)
            {
                _tcpClient.Close();
 
            }
            _cancellationTokenSource?.Cancel();
            isConnected = false;
            OnDisconnectHandle?.Invoke(this);
        }

        public void Dispatcher(ISession session, IMessage message)
        {
            if (message == null)
            {
                //logger.Error($"Error: recvived a null message");
                return;
            }
            var type = message.Type;
 
            switch (type)
            {
                case MessageType.NULL:
                    logger.Error($"Error: recvived a null message");
                    break;
                case MessageType.CTL:
                    MsgCtl msgCtl = message as MsgCtl;
                    logger.Info($"CTL Message:{msgCtl.CtlType}");
                    break;
                case MessageType.FrameInput:
                    MsgFrameInput msgFrameInput = message as MsgFrameInput;
                    _belongToRoom.ProcessInput(msgFrameInput);
                    break;
            }
        }
    }
}