using System;
using FS.Logic;
using FS.Model;
using UnityEngine;

namespace View
{
    public class PlayerView:VGameObject
    {
        private Player playerModel;
        
        public void SetModel(BaseEntity model)
        {
            playerModel = model as Player;
            if (playerModel == null)
            {
                Debug.LogError("PlayerView: SetModel failed, model is not a Player.");
            }
        }


        private void Update()
        {
            var pos = playerModel.transform.Position;
            transform.position = Vector3.Lerp(transform.position,
                new Vector3((float)pos.x,(float)pos.y,(float)pos.z), 0.3f);
        //TODO:改造成FVector3.Lerp


        }
    }
}