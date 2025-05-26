using FixMath.NET;
using FS.Math;
using FS.Model;
using UnityEngine.InputSystem;

namespace FS.Logic
{
    public class BaseComponent:IComponent
    {
        public BaseEntity entity;
        public FTransform transform;
        public virtual void BindEntity(BaseEntity entity){
            this.entity = entity;
            transform = entity.transform;
        }
        
        
        
        
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

        public virtual void DoDisable()
        {
             
        }

        public virtual void DoDestroy()
        {
            
        }

        public virtual void DoLateUpdate()
        {
             
        }
    }
    
    
    
    public abstract partial class BasePlayerComponent : BaseComponent {
        public Player player;
        public PlayerInputInfo input => player._inputInfos;

        public override void BindEntity(BaseEntity entity){
            base.BindEntity(entity);
            player = (Player) entity;
        }
    }
}