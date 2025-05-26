using System;
using FixMath.NET;

namespace FS.Logic
{
    [Serializable]
 
    public class BaseLifeCycle : ILifeCycle {
        public virtual void DoAwake()
        {
            
        }

        public virtual void DoEnable()
        {
            
        }

        public virtual void DoStart()
        {
            
        }

        public virtual void DoUpdate(Fix64 deltaTime)
        {
            
        }

        public virtual void DoDestroy()
        {
            
        }

        public virtual void DoDisable()
        {
            
        }


        public virtual void DoLateUpdate()
        {
            
        }
    }
}