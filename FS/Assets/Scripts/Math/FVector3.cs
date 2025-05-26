using FixMath.NET;
using FS.Network;
using MessagePack;
using UnityEngine;

namespace FS.Math
{
    
    
    [MessagePackObject]
    public struct FVector3:IFSSerializable
    {
        [Key(0)]
        public Fix64 x;
        [Key(1)]
        public Fix64 y;
        [Key(2)]
        public Fix64 z;


        public override string ToString()
        {
            return $"({x},{y},{z})";
        }

        private static readonly FVector3 zeroVector = new FVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);
        private static readonly FVector3 oneVector = new FVector3(Fix64.One, Fix64.One, Fix64.One);
        
        public static FVector3 Zero => zeroVector;
        public static FVector3 One => oneVector;
        
        public FVector3(Fix64 x, Fix64 y, Fix64 z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        
        
        public static FVector3 operator +(FVector3 a, FVector3 b)
        {
            return new FVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static explicit operator FVector3(Vector3 vector3)
        {
            Fix64 fx = (Fix64)vector3.x;
            Fix64 fy = (Fix64)vector3.y;
            Fix64 fz = (Fix64)vector3.z;
            return new FVector3( fx, fy,  fz);
        }
        
        
        public byte[] Serialize()
        {
            return MessagePackSerializer.Serialize(this);
        }
    }
}