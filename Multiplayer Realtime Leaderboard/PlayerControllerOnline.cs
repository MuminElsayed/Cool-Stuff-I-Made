using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//Handles the player's multiplayer data
public class PlayerControllerOnline : MonoBehaviourPunCallbacks
{
    public float playerSpeed, playerDistance;
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>(); //Get the local player controller script
    }

    void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            //Update the player's speed and distance
            playerSpeed = playerController.MoveSpeed;
            playerDistance = playerController.DistanceElapsed;
            MultiplayerManager.instance.UpdatePlayerStats(playerSpeed, playerDistance, false);
        }
    }

    public void AddDistance()
    {
        playerController.DistanceElapsed += 10;
    }
}
