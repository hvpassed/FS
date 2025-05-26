 

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace FS.Network
{
    
    public class Sender:ISend
    {
        private NetworkStream _networkStream;
        private readonly string ClassName;
        
        public Sender(NetworkStream networkStream)
        {
            _networkStream = networkStream;
            ClassName = GetType().Name;
        }
        public async ValueTask<bool> Send(byte[] data)
        {
 
            if (_networkStream == null || !_networkStream.CanWrite)
            {

#if SERVER
                Console.WriteLine($"[{ClassName}] Network stream is not writable");
#else
                //Console.WriteLine($"[{ClassName}] Network stream is not writable");
                //Debug.LogError($"[{ClassName}] Network stream is not writable");
#endif
                return false;
            }

            try
            {
                // 1. 生成协议头（4字节大端序长度）
                // byte[] header = BitConverter.GetBytes(data.Length);
                //
                //
                // // 2. 合并包头和内容
                // byte[] packet = new byte[header.Length + data.Length];
                // Buffer.BlockCopy(header, 0, packet, 0, header.Length);
                // Buffer.BlockCopy(data, 0, packet, header.Length, data.Length);

                // 3. 异步发送完整数据包
                await _networkStream.WriteAsync(data, 0, data.Length);
                await _networkStream.FlushAsync();
#if SERVER
                Console.WriteLine($"[{ClassName}] Sent {packet.Length} bytes");
#else
               // Console.WriteLine($"[{ClassName}] Sent {data.Length} bytes");
                //Debug.Log($"[{ClassName}] Sent {packet.Length} bytes");
                #endif
                return true;
            }
            catch (IOException ex)
            {

#if SERVER
                Console.WriteLine($"[{ClassName}] Network error: {ex.Message}");
#else
                //Console.WriteLine($"[{ClassName}] Network error: {ex.Message}");
                //Debug.LogError($"[{ClassName}] Network error: {ex.Message}");
#endif
                //Debug.LogError($"[{ClassName}] Network error: {ex.Message}");
 
                return false;
            }
            catch (ObjectDisposedException)
            {
#if SERVER
                Console.WriteLine($"[{ClassName}] Stream already disposed");
#else
                //Console.WriteLine($"[{ClassName}] Stream already disposed");
                //Debug.LogWarning($"[{ClassName}] Stream already disposed");
#endif
                //Debug.LogWarning($"[{ClassName}] Stream already disposed");
                return false;
            }
            catch (Exception ex)
            {
#if SERVER
                Console.WriteLine($"[{ClassName}] Send failed: {ex.Message}");
#else
               // Console.WriteLine($"[{ClassName}] Send failed: {ex.Message}");
                //Debug.LogError($"[{ClassName}] Send failed: {ex.Message}");
#endif
                //Debug.LogError($"[{ClassName}] Send failed: {ex.Message}");
                return false;
            }
        }
    }
}