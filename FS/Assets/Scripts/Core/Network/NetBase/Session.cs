using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FS.Utils;
using FS.Logic;
using FS.Manager;
using UnityEngine;

namespace FS.Network
{
    public class Session:ISession,IDisposable,IMessageDispatcher
    {



        private string ClassName = "";
        private int maxCount = 3;
        private int currentCount = 0;
        private NetworkStream _networkStream;
        private Receiver _receiver;
        private Sender _sender;
        private CancellationTokenSource _connectionCancellationTokenSource = new CancellationTokenSource();
        
        private NetworkManager _networkManager;
        private GameManager _gameManager;
        private TcpClient _tcpClient;
        public void CancelConnection()
        {
            if (_connectionCancellationTokenSource != null)
            {
                _connectionCancellationTokenSource.Cancel();
            }
        }

        public Session()
        {
            _networkManager = NetworkManager.Instance;
            _gameManager = GameManager.Instance;
            _tcpClient = new TcpClient();
            ClassName = GetType().Name;
        }
        
        public async ValueTask<bool>  Connect(string address, ushort port)
        {
            Debug.Log($"[{ClassName}]try to connect...");
            //ValueTask<bool> task = new ValueTask<bool>(false);
            await Task.Delay(1);
            currentCount++;
            if(currentCount > maxCount)
            {
                Debug.Log($"[{ClassName}]connect success");
                return true;
            }
            return false;
        }

        public async Task<bool> Connect( )
        {
 
            while (true)
            {
                try
                {
                    Debug.Log($"[{ClassName}] try to connect ..."); 
                    _connectionCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    await _tcpClient.ConnectAsync("127.0.0.1", 54321);
                    if (_tcpClient.Connected)
                    {
                        _networkStream = _tcpClient.GetStream();
                        _receiver = new Receiver(_networkStream);
                        _sender = new Sender(_networkStream);
                        Debug.Log($"[{ClassName}] Connected to server.");
                        return true;
                    }
                }
                catch (OperationCanceledException)
                {
                    Debug.Log($"[{ClassName}] Connection task canceled.");
                    return false;
                }
                catch (Exception ex)
                {
                    Debug.Log($"[{ClassName}] Connect exception: {ex.Message}. Retrying in 5 seconds...");
                    await Task.Delay(5000, _connectionCancellationTokenSource.Token);  // 同样支持取消
                }
            }
 
        }
        public void Disconnect()
        {
            
            _networkStream?.Close();
            
            _tcpClient?.Close();
            
        }

        // public async ValueTask<bool> Send(byte[] packet)
        // {
        //     try
        //     {
        //         if (_networkStream != null && _tcpClient.Connected)
        //         {
        //             await _networkStream.WriteAsync(packet, 0, packet.Length);
        //             Debug.Log($"[{ClassName}] Sent data: {BitConverter.ToString(packet)}");
        //             return true;
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Debug.LogError($"[{ClassName}] Send failed: {ex.Message}");
        //     }
        //     return false;
        // }

        // public async ValueTask<byte[]> Recv()
        // {
        //     try
        //     {
        //         if (_networkStream != null && _tcpClient.Connected)
        //         {
        //             byte[] buffer = new byte[1024];
        //             int bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length);
        //             Debug.Log($"[{ClassName}] Received data length: {bytesRead}");
        //             if (bytesRead > 0)
        //             {
        //                 byte[] data = new byte[bytesRead];  
        //                 Array.Copy(buffer, data, bytesRead);
        //                 Debug.Log($"[{ClassName}] Received data: {BitConverter.ToString(data)}");
        //                 return data;
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Debug.LogError($"[{ClassName}] Receive failed: {ex.Message}");
        //     }
        //     return Array.Empty<byte>();
        // }

        public async Task StartReceiving()
        {
            try
            {
                while (!_connectionCancellationTokenSource.Token.IsCancellationRequested && _tcpClient.Connected)
                {
                    byte[] data = await _receiver.Recv();
//                    Debug.Log($"Recevied {data.Length}");
//                    HexFileHelper.SaveBytesAsHexString(data, "recv.txt");
                    var message = MessageFactory.ParseMessage(data, out var messageType, false) as IMessage;
                    _networkManager.receivedPacket++;
                    //Debug.Log($"Received Message, {message.Type}");
                    Dispatcher(this, message);

                    
                    // if (data.Length > 0)
                    // {
                    //     // Handle received data
                    //     string content = Encoding.UTF8.GetString(data);
                    //     Debug.Log($"[{ClassName}] Processing received data... {content}");
                    //     if (content == "Hello")
                    //     {
                    //         byte [] response = Encoding.UTF8.GetBytes("Hello from client");
                    //         await _sender.Send(response);
                    //     }
                    //     
                    // }
                }
            }
            catch (IOException ex)
            {
                Debug.Log("Disconnect");
                Dispose();
            }
            catch (Exception ex)
            {
                Debug.LogError($"{ex.StackTrace}");
                Debug.LogError($"[{ClassName}] Receiving loop stopped: {ex.Message}");
            }
        }

        public async Task Send(byte[] data)
        {
            _networkManager.sendedPacket++;
            await _sender.Send(data);
        }

        public async Task Send(ISendable data)
        {
            _networkManager.sendedPacket++;
            await _sender.Send(MessageFactory.CreateMessage(data));
        }

        public void Dispose()
        { 
            CancelConnection();
            Disconnect();
        }

        public void Dispatcher(ISession session, IMessage message)
        {
            if (message == null)
            {
                Debug.LogError($"Error: recvived a null message");
                return;
            }
            var type = message.Type;
            switch (type)
            {
                case MessageType.NULL:
                    Debug.LogError($"Error: recvived a null message");
                    break;
                case MessageType.CTL:
                    Debug.Log($"Recvice: received a CTL message");
                    break;
                case MessageType.StartGame:
                    _gameManager.StartGame((MsgStartGame)message);
                    break;
                case MessageType.FrameInput:
                    break;
            }
        }
    }
}