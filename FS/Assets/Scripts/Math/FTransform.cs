using FixMath.NET;

namespace FS.Math
{
    public class FTransform
    {
        private FVector3 position;
        private FVector3 rotation;
        
        public FTransform(FVector3 position, FVector3 rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
        
        public FVector3 Position
        {
            get => position;
            set
            {
                position = value;
            }
        }
        
        public FVector3 Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
            }
        }

        public Fix64 x
        {
            get
            {
                return position.x;
            }
            set
            {
                position.x = value;
            }
        }
        
        public Fix64 y
        {
            get
            {
                return position.y;
            }
            set
            {
                position.y = value;
            }
        }
        public Fix64 z
        {
            get
            {
                return position.z;
            }
            set
            {
                position.z = value;
            }
        }
    }
    
}