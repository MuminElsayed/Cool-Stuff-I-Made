using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System;
using Photon.Realtime;
using System.Linq;

//Class for handling the realtime leaderboard for each room
public class RealtimeLeaderboard : MonoBehaviourPunCallbacks
{
    private List<Player> PlayersList { get; set; } = new List<Player>();
    [SerializeField]
    private LeaderboardEntry PlayerEntryPrefab;
    public static Action<Player> UpdatePlayerStats, RemovePlayerEvent;

    public override void OnJoinedRoom() //Called when joined an online room
    {
        base.OnJoinedRoom(); 
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayer(otherPlayer); //Remove the other player
    }
    public override void OnLeftRoom() //Called when leaving an online room (didn't work)
    {
        RemovePlayer(PhotonNetwork.LocalPlayer);
    }

    //Called when player stats are updated over the Photon network (only works when joined a room)
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedStats)
    {
        UpdateLeaderboard(targetPlayer); //Update the leaderboard UI with the new values
    }

    void AddNewPlayer(Player player) //Add a new player to the leaderboard player list, and create a UI entry for him
    {
        LeaderboardEntry entry = Instantiate(PlayerEntryPrefab, transform); //Create a new entry in the leaderboard
        entry.PlayerID = player; //Set the player reference to the new entry
        PlayersList.Add(player); //Add to the players list
    }

    public void RemovePlayer(Player player)
    {
        PlayersList.Remove(player); //Remove from the players list
        RemovePlayerEvent?.Invoke(player); //Call the remove event to update the UI
    }

    void UpdateLeaderboard(Player targetPlayer) //Update the values in the leaderboard
    {
        if (!PlayersList.Contains(targetPlayer)) //If can't find player in our playerList, add it
        {
            AddNewPlayer(targetPlayer);
        }

        //Update the UI
        UpdatePlayerStats?.Invoke(targetPlayer); //Send the update action for the UI with the current player (if it exists)
    }
}
