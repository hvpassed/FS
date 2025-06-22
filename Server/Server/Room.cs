using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using FS.Logic;
using FS.Math;
using FS.Model;
using FS.Network;
using NLog;

namespace FS.Server
{
    struct PlayerInfo
    {
        public ClientSession session;
        public int playerId;
        public int sessionId;
    }
    
    
    public class Room
    {
 
        List<PlayerInfo> players = new List<PlayerInfo>(MaxPlayer);
        public HashSet<int> playerId = new HashSet<int>();

        private MsgFrameInput _msgFrameInput;
        private static SpinLock _spinLock = new SpinLock();
        public int playerCount = 0;

        public const int MaxPlayer = 2;

        private static Logger logger = LogManager.GetCurrentClassLogger();
    
        private bool isStart = false;

        private long frame = 0;
        public long Frame => frame;
        private MsgFrameInput broadcastFrameInput;
        //private PlayerInputInfo[] nextFrameOpt = new PlayerInputInfo[MaxPlayer];//当前帧玩家输入
        private bool[] recvedInputMsg = new bool[MaxPlayer];
        private bool needClear = false;
        private Dictionary<long,PlayerInputInfo[]> tick2PlayerInfo = new Dictionary<long, PlayerInputInfo[]>();//玩家输入记录
        public Room()
        {
            players.Capacity = MaxPlayer;
            for (int i = 0; i < MaxPlayer; i++)
            {
                players.Add(new PlayerInfo
                {
                    session = null,
                    playerId = -1,
                    sessionId = -1
                });
            }

            for (int i = 0; i < MaxPlayer; i++)
            {
                recvedInputMsg [i] = false;
            }
            _msgFrameInput = new MsgFrameInput();
            broadcastFrameInput = new MsgFrameInput()
            {
                frameId = 0,
                playerId = -1,
                inputCount = 0,
                inputs = null,
            };
            //nextFrameOpt.Capacity = MaxPlayer;
            // for (int i = 0; i < MaxPlayer; i++)
            // {
            //     nextFrameOpt[i] = (new PlayerInputInfo()
            //     {
            //         keyboardInput = FVector2.Zero,
            //         mouseInput = FVector2.Zero
            //     });
            // }
        }

        public void DoUpdate()
        {
            var lockTaken = false;
            if (isStart)
            {
                //if(inputMgsRecvCount != playerCount){return;}
                try
                {

                    _spinLock.Enter(ref lockTaken);
                    //logger.Warn("[Doupdate]Get Lock");
                    if (!tick2PlayerInfo.TryGetValue(frame, out var toSend))
                    {
                        return;
                    }
                    needClear = true;
                    
                    
                    // broadcastFrameInput.inputCount = playerCount;
                    // broadcastFrameInput.frameId = frame;
                    // //broadcastFrameInput.inputs = new PlayerInputInfo[MaxPlayer];
                    // for (int i = 0; i < MaxPlayer; i++)
                    // {
                    //     broadcastFrameInput.inputs[i] = nextFrameOpt[i];
                    // }
                    // logger.Info("frame aregg cmpt:\n");
                    // for (int i = 0; i < MaxPlayer; i++)
                    // {
                    //     PlayerInputInfo playerInputInfo = broadcastFrameInput.inputs[i];
                    //     
                    //     logger.Info($"{i} {playerInputInfo.keyboardInput} {playerInputInfo.mouseInput}");
                    // }
                    // for (int i = 0; i < players.Count; i++)
                    // {
                    //     
                    //     if (players[i].playerId >= 0)
                    //     {
                    //         //_msgFrameInput.delteTime = 0;
                    //         // _msgFrameInput.playerId = players[i].playerId;
                    //         // _msgFrameInput.frameId = frame;
                    //         // //logger.Info($"Send to {players[i].playerId}");
                    //         logger.Info($"Send frame input to player {players[i].playerId} session {players[i].sessionId}");
                    //         players[i].session.Send(broadcastFrameInput);
                    //     }
                    // }
                    broadcastFrameInput.frameId = frame;
                    broadcastFrameInput.inputCount = playerCount;
                    broadcastFrameInput.inputs = toSend;
                    logger.Info($"frame send {frame}\n");
                    BoardCastFrameInput(broadcastFrameInput);
                    
                }
                finally
                {
                    if(lockTaken){
                        _spinLock.Exit();
                       // logger.Warn("[Doupdate]Release Lock");
                    }
                    
                }


                //logger.Info($"Logic Frame : {frame}");
            }
                            

        }

        public void DoLateUpdate()
        {
            if (isStart&&needClear)
            {
                bool lockTaken = false;
                try
                {
                    _spinLock.Enter(ref lockTaken);
                    needClear = false;
                    //logger.Warn("[DoLateUpdate]Get Lock");

                    tick2PlayerInfo.Remove(frame);
                    // for (int i = 0; i < MaxPlayer; i++)
                    // {
                    //     nextFrameOpt[i] = new PlayerInputInfo()
                    //     {
                    //         keyboardInput = FVector2.Zero,
                    //         mouseInput = FVector2.Zero
                    //     };
                    // }

                    //inputMgsRecvCount = 0;
                }
                finally
                {
                    if (lockTaken)
                    {
                        _spinLock.Exit();
                        //logger.Warn("[DoLateUpdate]Release Lock");

                    }

                }


                frame++;
            }
        }
        
        public void AddPlayer(ClientSession session, int sessionId)
        {
            if(playerCount >= MaxPlayer)
            {
                session.Send(new MsgCtl()
                {
                    CtlType = NetworkCtlType.RST
                });
                logger.Info("Room is full");
                session.Disconnect();
                return;
            }
            
            int id = 3;
            for (int i = 0; i < MaxPlayer; i++)
            {
                if (!playerId.Contains(i))
                {
                    id = i;
                    break;
                }
            }

            playerId.Add(id);
            
            playerCount++;
            if (!isStart)
            {
                frame = 0;
                isStart = true;
            }
            logger.Info($"Add player {id} to room {sessionId}");
            players[id] = new PlayerInfo()
            {
                session = session,
                playerId = id,
                sessionId = sessionId
            };
            logger.Info($"Player {session.GetRemoteAddress()} join room {id}");
            session.OnDisconnectHandle += DoExit;
            session.DoAwake();
            session.DoStart(this);
            MsgStartGame msg = new MsgStartGame()
            {
                playerId = id,
                playerCount = playerCount
            };
            
            if (isStart)
            {
                StartGame(session,msg);
            }
            
        }


        public void StartGame(ClientSession session,in MsgStartGame msgStartGame)
        {
            session.Send(msgStartGame);
            //session.Send(msgStartGame);
        }

        public void ProcessInput(MsgFrameInput msg)
        {

            bool lockTaken = false;
            try
            {
                _spinLock.Enter(ref lockTaken);

                if (tick2PlayerInfo.TryGetValue(msg.frameId, out var inputInfos))
                {   
                    inputInfos[msg.playerId] = msg.inputs[0];
                    
                }
                else
                {
                    PlayerInputInfo[] infos = new PlayerInputInfo[MaxPlayer];
                    tick2PlayerInfo.Add(msg.frameId,infos);
                    infos[msg.playerId] = msg.inputs[0];
                }
                //logger.Warn("[ProcessInput]Get Lock");
 
                // nextFrameOpt[msg.playerId] = new PlayerInputInfo()
                // {
                //     keyboardInput = msg.inputs[0].keyboardInput,
                //     mouseInput = msg.inputs[0].mouseInput,
                // };
            }
            finally
            {
                if (lockTaken)
                {
                    _spinLock.Exit();
                    //logger.Warn("[ProcessInput]Release Lock");
                }
            }

            // StringBuilder sb = new StringBuilder();
            // logger.Info($"Player Actions {msg.inputCount}");
            // for (int i = 0; i < msg.inputCount; i++)
            // {
            //     PlayerInputInfo inputInfo = msg.inputs[i];
            //     //sb.Append($" -> ({inputInfo?.keyboardInput}, {inputInfo?.mouseInput}) ");
            // }
            // logger.Info($"Player {playerId} input: {sb.ToString()}");
        }
        
        public void BoardCastFrameInput(MsgFrameInput infos)
        {

            foreach (var id in playerId)
            {
                players[id].session.Send(infos);
            }
            
            
        }
        
        public void DoExit(ClientSession clientSession)
        {
            bool find = false;
            int id = 0;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].session == clientSession)
                {
                    id = players[i].playerId;
                    players[i] = new PlayerInfo()
                    {
                        session = null,
                        playerId = -1,
                        sessionId = -1
                    };
                    
                    find = true;
                    break;
                }
            }

            if (find)
            {
                logger.Info($"Player: {id} exit.");
                playerId.Remove(id);
                playerCount--;
                if (playerCount == 0)
                {
                    frame = 0;
                    isStart = false;
                }
            }
        }

    }
}