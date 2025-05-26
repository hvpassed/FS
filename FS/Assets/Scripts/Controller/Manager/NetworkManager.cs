using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FixMath.NET;
using FS.CORE;
using FS.Logic;
using FS.Model;
using FS.Network;
using UnityEngine;

namespace FS.Manager
{
    public class NetworkManager:MonoBaseManager
    {
        private string ClassName = "";
        private static NetworkManager _instance;
        public static NetworkManager Instance => _instance;
        private GameManager _gameManager;
        private Session _session;
 
        private Task<bool> connect;
 
        private List<MsgCtl> msgCtls = new List<MsgCtl>();
        private bool startOnce = false;
        [NonSerialized]
        public long sendedPacket=0, receivedPacket=0;
        
        private FSPlayerInputManager _playerInputManager;
        private MsgFrameInput msgFrameInput;
        
        public override void DoAwake()
        {
            
            _instance = this;

            msgFrameInput = new MsgFrameInput();
            ClassName = GetType().FullName;
            Debug.Log($"[{ClassName}] Awake");
        }
 
        public override void DoStart()
        {
            _gameManager = GameManager.Instance;
            _playerInputManager = FSPlayerInputManager.Instance;
            Debug.Log($"[{ClassName}] Start");
            _session = new Session();
            connect = _session.Connect();

 
        }


        public void Send(ISendable messageFormater)
        {
            var message = MessageFactory.CreateMessage(messageFormater);
            _session.Send(message);
        }

        public void Update()
        {

            //ProcessMsgCtl();
            if (!_gameManager.hasStart)
            {
                if (connect is { IsCompletedSuccessfully: true, Result: true } && !startOnce)
                {
                     
 
                    Debug.Log($"[{ClassName}] NetworkManager Connect Success");
                    Debug.Log($"[{ClassName}] GameManage start to update");
 
                    _session.StartReceiving();
                    startOnce = true;
                }
            }
            
 

        }

        public override void DoUpdate(Fix64 deltaTime)
        {
            _playerInputManager._DoUpdate(deltaTime);
            // msgFrameInput.playerId = _gameManager.PlayerId;
            // msgFrameInput.frameId = _gameManager.frameCount;
            // //msgFrameInput.delteTime = deltaTime;
            // _session.Send(msgFrameInput);
            PlayerInputInfo  currentFrameInfo = _playerInputManager.GetCurrentFrameInfo(out int inputCount);
            _session.Send(new MsgFrameInput(_gameManager.PlayerId, _gameManager.FrameCount, currentFrameInfo, inputCount));
            
            
            
            //_playerInputManager.DoOnLateUpdate();
        }

        private void ProcessMsgCtl()
        {
            if (msgCtls.Count != 0)
            {
                foreach (MsgCtl ctl in msgCtls)
                {
                    switch (ctl.CtlType)
                    {
                        case NetworkCtlType.SYN:
                            Debug.Log($"[{ClassName}] Rececive message SYN-- start game");
                            _gameManager.hasStart = true;
                            break;
                        default:
                            break;
                    }
                }
                msgCtls.Clear();
            }

        }
        
        public void AddMsgCtl(MsgCtl msgCtl)
        {
            msgCtls.Add(msgCtl);
        }
        private void OnApplicationQuit()
        {
            Debug.Log($"[{ClassName}] NetworkManager DoDestroy");
            
            _session.Dispose();
        }

 
    }
}