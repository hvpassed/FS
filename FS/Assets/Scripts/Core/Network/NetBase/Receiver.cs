using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using IOException = System.IO.IOException;

namespace FS.Network
{
    public class Receiver:IReceive
    {
        private readonly NetworkStream _networkStream;
        private readonly string ClassName;
        private readonly byte[] _headerBuffer = new byte[4];
        private List<byte> _messageBuffer = new List<byte>();
        private int _expectedLength = -1;

        public Receiver(NetworkStream stream)
        {
            _networkStream = stream;
            ClassName = GetType().Name;
        }
        
        public async ValueTask<byte[]> Recv()
        {
            try
            {
                if (_networkStream == null || !_networkStream.CanRead)
                    return Array.Empty<byte>();

                // 1. 读取消息头（4字节长度）
                byte[] headerBuffer = await ReadBytesAsync(4);
                if (headerBuffer == null || headerBuffer.Length != 4)
                    throw new IOException();

                // 处理大端序（假设发送方是大端序） 
 
                int expectedLength = BitConverter.ToInt32(headerBuffer, 0);

                // 2. 读取消息体
                var ret = await ReadBytesAsync(expectedLength-4);
                if (ret.Length == 0)
                {

                    throw new IOException();
                }

                return ret;
            }
            catch (IOException ex) 
            {
                //Debug.Log("连接被对方重置");
                throw;

            }
            catch (Exception ex)
            {
                //Debug.LogError($"[{_className}] Receive error: {ex.Message}");
                return Array.Empty<byte>();
            }
        }

        private async ValueTask<byte[]> ReadBytesAsync(int count)
        {
            byte[] buffer = new byte[count];
            int totalRead = 0;

            while (totalRead < count)
            {
                int bytesRead = await _networkStream.ReadAsync(
                    buffer, 
                    totalRead, 
                    count - totalRead);
            
                if (bytesRead == 0) break;
                totalRead += bytesRead;
            }
            return totalRead == count ? buffer : Array.Empty<byte>();
        }
        
        // public async ValueTask<byte[]> Recv()
        // {
        //     try
        //     {
        //         if (_networkStream == null || !_networkStream.CanRead)
        //             return Array.Empty<byte>();
        //
        //         // 分阶段处理：1.读取消息头 2.读取消息体
        //         while (true)
        //         {
        //
        //             // 阶段1：读取消息头
        //             if (_expectedLength == -1)
        //             {
        //                 await ReadHeaderAsync();
        //                 if (_expectedLength <= 0) return Array.Empty<byte>();
        //             }
        //
        //             // 阶段2：读取消息体
        //             var result = await ReadBodyAsync();
        //             if (result != null) return result;
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         //Debug.Log($"[{ClassName}] Receive error: {ex.Message}");
        //
        //         return Array.Empty<byte>();
        //     }
        // }
        //
        // private async ValueTask ReadHeaderAsync()
        // {
        //     // 确保读取完整的4字节消息头
        //     int headerBytesRead = 0;
        //     while (headerBytesRead < 4)
        //     {
        //         var bytesRead = await _networkStream.ReadAsync(
        //             _headerBuffer, 
        //             headerBytesRead, 
        //             4 - headerBytesRead);
        //         
        //         if (bytesRead == 0) break;
        //         headerBytesRead += bytesRead;
        //     }
        //
        //     // 处理Java的大端序字节序
        //
        //     _expectedLength = BitConverter.ToInt32(_headerBuffer, 0);
        //     Array.Clear(_headerBuffer, 0, 4); // 清空头缓存
        // }
        //
        // private async ValueTask<byte[]> ReadBodyAsync()
        // {
        //     // 动态调整接收缓冲区
        //     var buffer = new byte[Math.Min(_expectedLength, 4096)];
        //     int totalBytesRead = _messageBuffer.Count;
        //
        //     while (totalBytesRead < _expectedLength)
        //     {
        //         var bytesRead = await _networkStream.ReadAsync(
        //             buffer, 
        //             0, 
        //             Math.Min(buffer.Length, _expectedLength - totalBytesRead));
        //         
        //         if (bytesRead == 0) break;
        //
        //         _messageBuffer.AddRange(buffer.Take(bytesRead));
        //         totalBytesRead += bytesRead;
        //     }
        //
        //     // 判断是否接收完整
        //     if (totalBytesRead >= _expectedLength)
        //     {
        //         var result = _messageBuffer.Take(_expectedLength).ToArray();
        //         _messageBuffer.Clear();
        //         _expectedLength = -1;
        //         return result;
        //     }
        //
        //     return null;
        // }
    }
}