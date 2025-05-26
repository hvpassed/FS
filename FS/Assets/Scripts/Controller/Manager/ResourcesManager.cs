using System;
using FS.CORE;
using FS.Setting;
using UnityEngine;

namespace FS.Manager
{
    
    
    public class ResourcesManager:MonoBaseManager
    {
        private static ResourcesManager _instancec = null;
        public static ResourcesManager Instance => _instancec;

        private string ClassName = "";
        [NonSerialized]
        public LogicSetting logicSetting;
        
        
        public Action allLoadedAction;
        public override void DoAwake()
        {             
            ClassName = GetType().FullName;
            Debug.Log($"[{ClassName}] Awake");

            _instancec = this;
            GameManager.Instance.AllStartHandle += () =>
            {
                Debug.Log($"[{ClassName}] Resource Start Load Phase Complete");
                allLoadedAction?.Invoke();
            };
        }

        public override void DoStart()
        {
            Debug.Log($"[{ClassName}] Start");
            Debug.Log($"[{ClassName}] Loading Resources...");
            logicSetting = Resources.Load<LogicSetting>("Settings/LogicSetting");
             
        }
    }
}