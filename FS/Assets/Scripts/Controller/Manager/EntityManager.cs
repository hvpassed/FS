using System.Collections.Generic;
using FixMath.NET;
using FS.Logic;
using FS.Math;
using FS.Model;
using UnityEngine;

namespace FS.Manager
{
    public class EntityManager:MonoBaseManager
    {
        private string ClassName = "";
        private static EntityManager _instance;
        public static EntityManager Instance => _instance;
        public static List<Player> allPlayers = new List<Player>();
        public int currentPlayerId = 0;
        private NetworkManager _networkManager;
        private GameManager _gameManager;

        public override void DoAwake()
        {
            ClassName = GetType().Name;
            _instance = this;
        }


        public override void DoStart()
        {
            _gameManager = GameManager.Instance;
            _networkManager = NetworkManager.Instance;
        }


        public void AddUser() 
        {
            
            
        }


        public void StartGame(MsgStartGame msgStartGame)
        {
            allPlayers.Clear();
            for(int i = 0;i<msgStartGame.playerCount;i++)
            {
                Player np = new Player(msgStartGame.playerId);
                
                Debug.Log($"[{ClassName}] create player {msgStartGame.playerId}");
                np.transform.Position = new FVector3(0, 0, 0);
                allPlayers.Add(np);
            }

            currentPlayerId = msgStartGame.playerId;
        }

        public override void DoUpdate(Fix64 deltaTime)
        {
            UpdateFrameInput();
            foreach (Player player in allPlayers)
            {
                player.DoUpdate(deltaTime);
            }
        }

        public void UpdateFrameInput()
        {
            var currentFrameInput = _networkManager.GetCurrentFrameInput();
            if (currentFrameInput == null)
            {
                Debug.LogWarning($"[{ClassName}] No current frame input found.");
                return;
            }

            foreach (Player player in allPlayers)
            {
                if(currentFrameInput!=null)
                {
                    if (player == null)
                    {
                        Debug.LogError($"[{ClassName}] Plyar {currentPlayerId} is null.");
                    }
                    else
                    {

                        player.InputInfos.keyboardInput = currentFrameInput.keyboardInput;
                    }
                }
            }
        }
    }
}