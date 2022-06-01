using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

//This is the player entry UI for the leaderboard, each entry holds one player with his stats,
//and updates its values when that player's stats change (called from Photon's OnPlayerPropertiesUpdate())
public class LeaderboardEntry : MonoBehaviour
{
    private Player playerID; //Saves the player reference
    public Player PlayerID //Encapsulate
    { 
        get { return playerID; }
        set { playerID = value; nameText.text = value.ToString(); } //Set the value and the UI text, since it will be set only once at initilization.
    }

    public bool IsFinishedRace { get; private set; }
    [SerializeField]
    private TextMeshProUGUI nameText, distanceText; //Text fields

    private void Start()
    {
        UpdateValues(PlayerID); //Set values on start
    }

    private void UpdateValues(Player player)
    {
        if (player == PlayerID) //Check if this is the player is being updated
        {
            //Update the UI
            nameText.text = player.CustomProperties["PlayerName"].ToString();
            distanceText.text = player.CustomProperties["PlayerDistance"].ToString();
        }
    }

    private void DeleteEntry(Player player) //Remove player data and UI if he leaves/disconnects to avoid duplicates or dead data
    {
        if(player == playerID)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        //Subscribe to the stats update events
        RealtimeLeaderboard.UpdatePlayerStats += UpdateValues;
        RealtimeLeaderboard.RemovePlayerEvent += DeleteEntry;
    }

    private void OnDisable()
    {
        //Unsubscribe to the stats update events
        RealtimeLeaderboard.UpdatePlayerStats -= UpdateValues;
        RealtimeLeaderboard.RemovePlayerEvent -= DeleteEntry;
    }
}