using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public static MultiplayerManager instance;
    [SerializeField]
    private TMP_InputField createRoomID, joinRoomID;
    [SerializeField]
    private GameObject loadingMenu, lobbyMenu;
    private ExitGames.Client.Photon.Hashtable playerData;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //Sets player data that will be synced across the network (name, speed, distance, finishedRace)
        GameManager.instance.PlayerName = "player1";
        playerData = new ExitGames.Client.Photon.Hashtable() 
        { 
            {"PlayerName", GameManager.instance.PlayerName}, 
            {"PlayerSpeed", 0}, 
            {"PlayerDistance", 0},
            {"FinishedRace", false}
        };

        PhotonNetwork.ConnectUsingSettings(); //Connect to the Photon network using default settings
    }

    public void UpdatePlayerStats(float speed, float distance, bool isFinishedRace) //Update and cache stats locally
    {
        //Update the player's speed and distance
        playerData["PlayerName"] = GameManager.instance.PlayerName;
        playerData["PlayerSpeed"] = speed;
        playerData["PlayerDistance"] = distance;
        playerData["FinishedRace"] = isFinishedRace;
    }

    void SendPlayerStats() //Send stats to the network
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerData);
        }
    }

    public override void OnConnectedToMaster() //Called when connected to the Photon network
    {
        PhotonNetwork.LocalPlayer.NickName = GameManager.instance.PlayerName;
        PhotonNetwork.JoinLobby();
        Debug.Log("Connected to Photon Master network");
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.SetPlayerCustomProperties(playerData);
        //Got to main lobby
        Debug.Log("Connected to Lobby");
    }

    public void CreateRoom()
    {

        PhotonNetwork.CreateRoom(createRoomID.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoomID.text);
    }

    public override void OnCreatedRoom()
    {
        //Player is the "host", so send to map options screen
        //Changed multiplayer model to join seperate rooms, player distance will be synced through a realtimeleaderboard
        Debug.Log($"Succesfully created room {createRoomID.text}");
    }

    public override void OnJoinedRoom()
    {
        //Player is a normal user, send to waiting for host screen
        Debug.Log($"Succesfully joined room {joinRoomID.text}");


        //Send player stats to the server on a fixed interval (can be called after player starts the game)
        InvokeRepeating(nameof(SendPlayerStats), 1f, 1f);

        loadingMenu.SetActive(false);
        lobbyMenu.SetActive(true);
    }
}