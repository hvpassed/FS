using FixMath.NET;

namespace FS.Manager
{
    public class EntityManager:MonoBaseManager
    {
        private string ClassName = "";
        private static EntityManager _instance;
        public static EntityManager Instance => _instance;
        

        public override void DoAwake()
        {
            ClassName = GetType().Name;
            _instance = this;
        }


        public override void DoStart()
        {
            
        }


        public override void DoUpdate(Fix64 deltaTime)
        {
            
        }
    }
}