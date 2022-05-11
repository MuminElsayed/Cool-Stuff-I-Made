using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Save player data locally using JSON and PlayerPrefs
public class SavedPlayerData : MonoBehaviour
{
    public static savedPlayerData instance;

    //Data to save
    public string playerName {get; set;} = "Player123";
    public int playerSkinNum {get; set;} = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void OnEnable()
    {
        //Load player data
        JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("savedPlayerData"), this);
    }

    void OnDisable()
    {
        //Save player data
        string jsonData = JsonUtility.ToJson(this, false);
        PlayerPrefs.SetString("savedPlayerData", jsonData);
        PlayerPrefs.Save();
    }
}
