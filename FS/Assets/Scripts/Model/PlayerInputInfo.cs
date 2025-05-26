using FS.Math;
using FS.Network;
using MessagePack;

namespace FS.Model
{
    [MessagePackObject]
    public class PlayerInputInfo:IFSSerializable
    {
        [Key(0)]
        public FVector2 keyboardInput;
        [Key(1)]
        public FVector2 mouseInput;

 

        public byte[] Serialize()
        {
            return MessagePackSerializer.Serialize(this);
        }
    }
}