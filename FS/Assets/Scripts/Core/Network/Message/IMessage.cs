using System;

namespace FS.Network
{
 
    
    
    public interface IMessage
    {
 
        MessageType Type { get;  }
    }
    
    public interface IFSSerializable
    {
        byte[] Serialize();
        //T Deserialize(byte[] data);
    }
    
    public interface IFSSerializablePacket
    {
        byte[] ToBytes();
        void FromBytes(byte[] data);
 
    }
    // [Serializable]
    // public  class BaseFormater : ISerializable, ISerializablePacket {
    //     public virtual void Serialize(Serializer writer){ }
    //
    //     public virtual void Deserialize(Deserializer reader){ }
    //
    //     public override string ToString(){
    //         return JsonUtil.ToJson(this);
    //     }
    //
    //     public byte[] ToBytes(){
    //         var writer = new Serializer();
    //         Serialize(writer);
    //         var bytes = writer.CopyData(); // Compressor.Compress(writer.CopyData());
    //         return bytes;
    //     }
    //     public void FromBytes(byte[] data){
    //         var bytes = data; //Compressor.Decompress(data);
    //         var reader = new Deserializer(bytes);
    //         Deserialize(reader);
    //     }
    //     public void FromBytes(byte[] data,int offset,int size){
    //         var bytes = data; //Compressor.Decompress(data);
    //         var reader = new Deserializer(bytes,offset,size);
    //         Deserialize(reader);
    //     }
    //
    //     public static T FromBytes<T>(byte[] data) where T : BaseFormater, new(){
    //         var ret = new T();
    //         ret.FromBytes(data,0,data.Length);
    //         return ret;
    //     }
    //     public static T FromBytes<T>(byte[] data,int offset,int size) where T : BaseFormater, new(){
    //         var ret = new T();
    //         ret.FromBytes(data,offset,size);
    //         return ret;
    //     }
    // }
}