using FixMath.NET;
using FS.Math;

namespace FS.Logic
{
    public class FMove:BasePlayerComponent
    {
        public Fix64 speed => entity.speed;
        
        
        public override void DoUpdate(Fix64 deltaTime){
            //if (!entity.rigidbody.isOnFloor) {
            //    return;
            //}

            var needChase = input.keyboardInput.magnitude > Fix64.FromRaw(10);
            if (needChase) {
                var dir = input.keyboardInput.normalized;
                var xzSpeed = dir * speed * deltaTime;
                transform.Position += new FVector3(xzSpeed.x,0,xzSpeed.y);
                // var targetDeg = dir.ToDeg();
                // transform.Rotation = CTransform2D.TurnToward(targetDeg, transform.deg, 360 * deltaTime, out var hasReachDeg);
            }
 
        }
    }
}