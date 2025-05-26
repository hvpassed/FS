using FixMath.NET;
using FS.Network;
using MessagePack;
using UnityEngine;

namespace FS.Math
{
    [MessagePackObject]
    public struct FVector2:IFSSerializable
    {
        [Key(0)]
        public Fix64 x;
        [Key(1)]
        public Fix64 y;


        public override string ToString()
        {
            return $"({x},{y})";
        }

        private static readonly FVector2 zeroV = new FVector2(Fix64.Zero, Fix64.Zero);
        public static FVector2 Zero
        {
            get
            {
                return zeroV;
            }
        }
        private static readonly FVector2 oneV = new FVector2(Fix64.One, Fix64.One);
        public static FVector2 One
        {
            get
            {
                return oneV;
            }
        }
        public FVector2(Fix64 x, Fix64 y)
        {
            this.x = x;
            this.y = y;
        }

        public static FVector2 operator *(FVector2 a, Fix64 b)
        {
            return new FVector2(a.x * b , a.y * b);
        }
        
        
        public static explicit operator FVector2(Vector2 vector)
        {
           Fix64 x = (Fix64)vector.x;
           Fix64 y = (Fix64)(vector.y);
 
           return new FVector2(x, y);
        }
        
        [IgnoreMember]
        public FVector2 normalized
        {
            get
            {
                Fix64 magnitude = Fix64.Sqrt(x * x + y * y);
                if (magnitude > Fix64.Zero)
                {
                    return new FVector2(x / magnitude, y / magnitude);
                }
                return Zero;
            }
        }


        [IgnoreMember]
        public Fix64 magnitude
        {
            get
            {
                return Fix64.Sqrt(x * x + y * y);
            }
        }
        public byte[] Serialize()
        {
            return MessagePackSerializer.Serialize(this);
        }
    }
}