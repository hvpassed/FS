using FixMath.NET;

namespace FS.Logic
{
    public interface IUpdate
    {
        void DoUpdate(Fix64 deltaTime);
    }
}