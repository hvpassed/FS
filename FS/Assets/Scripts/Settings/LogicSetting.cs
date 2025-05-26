using System;
using UnityEngine;

namespace FS.Setting
{
    [Serializable]
    [CreateAssetMenu(fileName = "LogicSetting", menuName = "FS/Settings/LogicSetting")]
    public class LogicSetting:ScriptableObject
    {
        public float LogicFPS = 30;
    }
}