using System;
using System.Collections.Generic;
using FixMath.NET;
using FS.CORE;
using FS.Logic;
using FS.Math;
using FS.Model;
using FS.Network;

using FS.Setting;
using MessagePack;
using UnityEngine;
using UnityEngine.Serialization;

namespace FS.Manager
{
    public class GameManager:MonoBaseManager 
    {

        #region Singleton


        
        private static GameManager _instance = null;

        private static object _lock = new object();

        public static GameManager Instance
        {
            get
            {
                
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            var existingInstance = FindObjectOfType<GameManager>();
                            if (existingInstance != null)
                            {
                                _instance = existingInstance;
                            }
                            else
                            {
                                var newGameObject = new GameObject("GameManager");
                                _instance = newGameObject.AddComponent<GameManager>();
                            }
                        }
                    }
                }
                return _instance;
            }
        }
                

        #endregion

        private List<MonoBaseManager> _mgr = new List<MonoBaseManager>();

        private   string ClassName;
        [NonSerialized]
        public bool hasStart = false;
        [NonSerialized]
        public float intervalTime = 0.033f;

        [NonSerialized]
        public float passTime = 0;

        [NonSerialized]
        private long frameCount = 0;

         public long FrameCount => frameCount;
        private ResourcesManager _resourcesManager;
        private NetworkManager _networkManager;
        
        public Action AllStartHandle;


        [NonSerialized]
        public int PlayerId;
         
        [NonSerialized]
        public int PlayerCount;

        private float lastStepTime = 0;

        #region  MonoLifeCycle
        

        private void Awake()
        {
            _Awake();
        }


        private void Start()
        {
            _Start();
        }

        private void Update()
        {
            _Update();
        }

        private void LateUpdate()
        {
            _LateUpdate();
        }

        private void OnDestroy()
        {
            _OnDestroy();
        }
                

        #endregion

        #region MyLife


        private void _Awake()
        {
            DoAwake();
            foreach (MonoBaseManager manager in _mgr)
            {
                manager?.DoAwake();
            }
            _resourcesManager = ResourcesManager.Instance;
            _networkManager = NetworkManager.Instance;
            
            _resourcesManager.allLoadedAction += () =>
            {
                intervalTime = 1f / _resourcesManager.logicSetting.LogicFPS;
            };

        }
        
        private void _Start()
        {
            DoStart();
            
            foreach (MonoBaseManager manager in _mgr)
            {
                manager?.DoStart(); 
            }
            Debug.Log($"[{ClassName}] All Manager Start");
            AllStartHandle?.Invoke();
        }

        private void _Update()
        {
            _DoUpdate();
        }

        private void _LateUpdate()
        {
            //_DoLateUpdate();
        }


        private void _OnDestroy()
        { 
 
            DoDestroy();

        }

        #endregion

        private void _DoLateUpdate()
        {

            foreach (MonoBaseManager manager in _mgr)
            {
                manager?.DoLateUpdate();
            }
            DoLateUpdate();

        }

        public override void DoLateUpdate()
        {
            frameCount++;
        }

        public override void DoAwake()
        {
 
            PlayerInputInfo playerInputInfo = new PlayerInputInfo()
            {
                mouseInput = FVector2.One,
                keyboardInput = FVector2.One
            };
            PlayerInputInfo[] playerInputInfos = new PlayerInputInfo[2];
            playerInputInfos[0] = playerInputInfo;
            playerInputInfos[1] = playerInputInfo;
            MsgFrameInput msgFrameInput = new MsgFrameInput()
            {
            
                frameId = 1,
                playerId = 1,
                inputs = playerInputInfos,
                inputCount =  2
            };
            var data = MessageFactory.CreateMessage(msgFrameInput);
            Debug.Log($"data {data.Length}");
            var message = MessageFactory.ParseMessage(data, out var t, true);
            Debug.Log($"message {t}");
            MsgFrameInput m2 = message as MsgFrameInput;
            Debug.Log($"MsgFrameInput {m2.frameId}  {m2.playerId}");
            Debug.Log($"MsgFrameInput {m2.inputs[0].keyboardInput}  {m2.playerId}");
            ClassName = GetType().FullName;
            Debug.Log($"[{ClassName}] Awake");
            _instance = this;
            MonoBaseManager[] monoBaseManagers = GetComponents<MonoBaseManager>();
            foreach (MonoBaseManager baseManager in monoBaseManagers)
            {
                if (baseManager != this)
                {
                    _mgr.Add(baseManager);
                }
            }
        }

        public override void DoStart()
        {
            Debug.Log($"[{ClassName}] Start");
        }
        
        private void _DoUpdate()
        {
            if(!hasStart) return;
            passTime += Time.deltaTime;
            if(passTime>intervalTime)
            {
                passTime -= intervalTime;
                //Debug.Log($"{Time.deltaTime}");

            
            
                
                
                
                Step();
                _DoLateUpdate();
            }
        }

        private void Step()
        {
            //Debug.Log($"[{ClassName}] Logic Step {frameCount++}");
 
 
            if (frameCount % 1000 == 0)
            {
 
                Debug.Log($"[{ClassName}] Logic Step {frameCount}");
            }
            
            
            Fix64 deltaTime = (Fix64)Time.deltaTime;
            foreach (MonoBaseManager manager in _mgr)
            {
                manager?.DoUpdate(deltaTime);
            }
        }

        public void StartGame(MsgStartGame msgStartGame)
        {
            Debug.Log($"[{ClassName}] Rececive message SYN-- start game");
            hasStart = true;
            PlayerId = msgStartGame.playerId;
            PlayerCount = msgStartGame.playerCount;
        }
    }
}