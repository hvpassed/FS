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


        public long syncFrame = 0;
        public long receivedFrame = 0;
        List<PlayerInputInfo> acceptedPlayerInputInfos = new List<PlayerInputInfo>(10000);
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
            Debug.Log($"[{ClassName}] GetCurrentFrameInfo {currentFrameInfo}");
            var frameInput = new MsgFrameInput(_gameManager.PlayerId, _gameManager.FrameCount, currentFrameInfo, inputCount);
            Debug.Log($"[{ClassName}] MsgFrameInput {frameInput.inputs[0]}");
            byte[] message = MessageFactory.CreateMessage(frameInput);
            MsgFrameInput des = MessageFactory.ParseMessage(message,out var type,true) as MsgFrameInput;
            Debug.Log($"[{ClassName}] processed MsgFrameInput {des.inputs[0]}");
            _session.Send(frameInput);
            
            
            
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

        public void PushMsgFrameInput(MsgFrameInput serverFrameInput)
        {
 
            //TODO:修改成保存全部
            //syncFrame = serverFrameInput.frameId;
            receivedFrame++;
            if (serverFrameInput.inputCount >= _gameManager.PlayerId)
            {
                //Debug.Log($"Recv input action : {serverFrameInput.inputs[_gameManager.PlayerId].keyboardInput}");
                acceptedPlayerInputInfos.Add(serverFrameInput.inputs[_gameManager.PlayerId]);
                Debug.Log($"Recv input action : {serverFrameInput.inputs[_gameManager.PlayerId].keyboardInput}, " +
                          $"{serverFrameInput.inputs[_gameManager.PlayerId].mouseInput}");
            }
 
            
        }
        
        
        public PlayerInputInfo GetCurrentFrameInput()
        {
            //TODO:随之修改成保存全部
            int current = (int)(syncFrame);
            if (current < 0 || current > receivedFrame)
            {
                Debug.LogWarning($"[{ClassName}] Getting frame:{current} with received {receivedFrame}");
                return PlayerInputInfo.Empty;
            }

            syncFrame++;
            return acceptedPlayerInputInfos[current];
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