using System;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using FS.Logic;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using UnityEngine;

namespace FS.Network
{
    
    public interface ISendable:IFSSerializable ,IMessage
    {
 
    }
    
    
    public class MessageFactory
    {
 
        /// <summary>
        /// 将消息体进行封装，生成一个完整的消息
        /// </summary>
        ///  
        /// <param name="info">消息体</param>
        /// <typeparam name="T">消息体类型，消息需要继承ISendable</typeparam>
        /// <returns></returns>
        public static byte[] CreateMessage<T>( T info) where T:ISendable
        {
            //TYPE占4个字节
            //TODO: 内存池优化
            int length = 4;
 
            byte[] typeBytes = BitConverter.GetBytes((int)(info.Type));
            //Debug.Log("Create with type: "+info.Type);
            byte[] bytes = info.Serialize();
            length += bytes.Length+4;
            byte[] lengthBytes = BitConverter.GetBytes(length);
            byte[] message = new byte[length];
            Buffer.BlockCopy(lengthBytes, 0, message, 0, 4);
            Buffer.BlockCopy(typeBytes, 0, message, 4, 4);
            Buffer.BlockCopy(bytes, 0, message, 8, bytes.Length);
            return message;
        }

        /// <summary>
        /// 对收到的消息进行解析，主要是去除头部的长度和类型信息
        /// </summary>
        /// <param name="recvData">收到的消息</param>
        /// <returns></returns>
        public static IMessage ParseMessage(byte[] recvData,out MessageType type,bool hasLength)
        {
            type = MessageType.UNKNOWN;
            if (recvData == null || recvData.Length < 4)
            {
                 
                return null;
            }
            //recv
            //长度占4个字节
            //类型占4个字节
            //消息体
            if (hasLength == false)
            {
                return ParseMessage(recvData,out type);
            }
            int offset = 0;
            int length = BitConverter.ToInt32(recvData,offset);
            offset += 4;
            type= (MessageType)BitConverter.ToInt32(recvData, offset);
            offset += 4;
            byte[] message = new byte[length - 8];
            Buffer.BlockCopy(recvData,offset,message,0,length-8);
            //Debug.Log($"Length: {length} Type: {type} ");

            return Parse(message,type);
        }
        
        public static IMessage ParseMessage(byte[] recvData,out MessageType type)
        {
            //recv
            //长度占4个字节
            //类型占4个字节
            //消息体
            int length = recvData.Length;
            int offset = 0;
            type= (MessageType)BitConverter.ToInt32(recvData, offset);
 
            //Console.WriteLine($"ParseMessage: {type}");
 
            offset += 4;
            byte[] message = new byte[length - offset ];
            Buffer.BlockCopy(recvData,offset,message,0,length-offset);
            //Console.WriteLine($"Length: {length} Type: {type} ");
 
            return Parse(message,type);
        }

        public static IMessage Parse(in byte[] data, in MessageType type)
        {
            switch (type)
            {
                case MessageType.NULL:
                    return null;
                    
                case MessageType.CTL :
                    return MessagePackSerializer.Deserialize<MsgCtl>(data);
                case MessageType.StartGame:
                    return MessagePackSerializer.Deserialize<MsgStartGame>(data);
                case MessageType.FrameInput:
                    return MessagePackSerializer.Deserialize<MsgFrameInput>(data);
                default:
                    return null;
            }
            
        }
    }
}