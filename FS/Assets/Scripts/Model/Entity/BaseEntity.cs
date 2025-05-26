using System.Collections.Generic;
using FixMath.NET;
using FS.Math;

namespace FS.Logic
{
    public class BaseEntity:BaseLifeCycle, IEntity
    {
        public FTransform transform;
        public FRigidbody rigidbody;
        public Fix64 speed = Fix64.FromRaw(5);
        protected List<BaseComponent> allComponents = new List<BaseComponent>();
        
        
        public BaseEntity()
        {
            transform = new FTransform(FVector3.Zero,FVector3.Zero);
            rigidbody = new FRigidbody();
            RegisterComponent(rigidbody);
        }
        protected void RegisterComponent(BaseComponent comp){
            allComponents.Add(comp);
            comp.BindEntity(this);
        }
        
        public virtual void DoAwake()
        {
            foreach (var comp in allComponents)
            {
                comp.DoAwake();
            }
        }

        public override void DoStart()
        {
            foreach (var comp in allComponents)
            {
                comp.DoAwake();
            }
        }
        
        public override void DoUpdate(Fix64 deltaTime)
        {
            foreach (var comp in allComponents)
            {
                comp.DoUpdate(deltaTime);
            }
        }
        
        public override void DoDestroy(){
            foreach (var comp in allComponents) {
                comp.DoDestroy();
            }
        }
    }
}