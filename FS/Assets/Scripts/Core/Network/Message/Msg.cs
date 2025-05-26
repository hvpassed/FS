using System;
using FixMath.NET;
using FS.Model;
using FS.Network;
using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;

namespace FS.Network
{
 
 
   public abstract class BaseMessageFormater: IFSSerializablePacket,ISendable
   {


      public abstract  byte[] Serialize();


      public abstract byte[] ToBytes();
      public abstract void FromBytes(byte[] data);

      public abstract  T FromBytes<T>(byte[] data);
      [IgnoreMember]
      public abstract MessageType Type { get; }
   }
}

namespace FS.Logic
{
   public enum NetworkCtlType : int
   {
      SYN=0,
      ACK=1,
      NAK=2,
      FIN=3,
      RST=4
   }
   
   
   [MessagePackObject]
   public class MsgCtl:BaseMessageFormater
   {
      public override byte[] Serialize()
      {
         return MessagePackSerializer.Serialize(this);
 
      }

      public override byte[] ToBytes()
      {
         return MessagePackSerializer.Serialize(this);
      }

      public override void FromBytes(byte[] data)
      {
         var msg = MessagePackSerializer.Deserialize<MsgCtl>(data) as MsgCtl;
         CtlType = msg.CtlType;
      }

      public  override MsgCtl FromBytes<MsgCtl>(byte[] data)
      {
         return MessagePackSerializer.Deserialize<MsgCtl>(data);
      }

      [IgnoreMember]
      public override MessageType Type =>MessageType.CTL;
      
      [Key(0)]
      public NetworkCtlType CtlType;

   }


   [MessagePackObject]
   public class MsgStartGame : BaseMessageFormater
   {
      public override byte[] Serialize()
      {
         return MessagePackSerializer.Serialize(this);
 
      }

      public override byte[] ToBytes()
      {
         return MessagePackSerializer.Serialize(this);
      }

      public override void FromBytes(byte[] data)
      {
         var msg = MessagePackSerializer.Deserialize<MsgStartGame>(data) as MsgStartGame;
         playerId = msg.playerId;
         playerCount = msg.playerCount;
      }

      public override MsgStartGame FromBytes<MsgStartGame>(byte[] data)
      {
         return MessagePackSerializer.Deserialize<MsgStartGame>(data);
      }

      [IgnoreMember]
      public override MessageType Type => MessageType.StartGame;
      
      [Key(0)]
      public int playerId;
      
      [Key(1)]
      public int playerCount;
      
   }

   [MessagePackObject]
   public class MsgFrameInput : BaseMessageFormater
   {


      public MsgFrameInput()
      {
         
      }
      public MsgFrameInput(int playerId, long frameId, PlayerInputInfo  data, int inputCount)
      {
         this.inputs = new PlayerInputInfo[inputCount];
         this.playerId = playerId;
         this.frameId = frameId;
         this.inputCount = inputCount;
         this.inputs[0] = data;
      }
 
      public override byte[] Serialize()
      {
         return MessagePackSerializer.Serialize(this );
 
      }

      public override byte[] ToBytes()
      {
         return MessagePackSerializer.Serialize(this );
      }

      public override void FromBytes(byte[] data)
      {
         var msg = MessagePackSerializer.Deserialize<MsgFrameInput>(data ) as MsgFrameInput;
         playerId = msg.playerId;
         frameId = msg.frameId;

      }

      public override MsgFrameInput FromBytes<MsgFrameInput>(byte[] data)
      {
         return MessagePackSerializer.Deserialize<MsgFrameInput>(data);
      }


      [IgnoreMember]
      public override MessageType Type => MessageType.FrameInput;

      
      [Key(0)]
      public int playerId;

      [Key(1)] public long frameId;

      [Key(2)] public PlayerInputInfo[] inputs;
      [Key(3)] public int inputCount;
   }
   
}