using System.Collections.Generic;
using FixMath.NET;
using FS.Logic;
using FS.Math;
using UnityEngine;
using View;

namespace FS.Model
{
    public class Player:BaseEntity
    {
        public PlayerInputInfo _inputInfos;

        public PlayerInputInfo InputInfos
        {
            get => _inputInfos;
            set => _inputInfos = value;
        }
        private int playerId;

        private FMove move;
       // private GameObject playerPrefab;

        private GameObject view;
        

        public Player(int pi)
        {
            // playerPrefab = Resources.Load<GameObject>("Player/Player");
            // if (playerPrefab == null)
            // {
            //     Debug.LogError("Player prefab not found!");
            // }
            move = new FMove();
            RegisterComponent(move);
            prefab =  Resources.Load<GameObject>("Player/Player");
            if (prefab == null)
            {
                Debug.LogError("Player prefab not found!");
            }

            _inputInfos = new PlayerInputInfo()
            {
                keyboardInput = FVector2.Zero,
                mouseInput = FVector2.Zero
            };
            GameObject viewOj = GameObject.Instantiate(prefab);
            RegisterView(viewOj);
            playerId = pi;
            PlayerView playerView = view.GetComponent<PlayerView>();
            if (playerView != null)
            {
                playerView.SetModel(this);
            }
            else
            {
                Debug.LogError("PlayerView component not found on the prefab!");
            }
        }

        public override void RegisterView(GameObject viewPrefab)
        {
            this.view = viewPrefab;
        }

        
        
        // public void Regis()
        // {
        //     GameObject instantiate = GameObject.Instantiate(playerPrefab);
        //     
        //     PlayerView playerView = instantiate.GetComponent<PlayerView>();
        //     if (playerView != null)
        //     {
        //         playerView.SetModel(this);
        //     }
        // }

 
    }
    
    
}