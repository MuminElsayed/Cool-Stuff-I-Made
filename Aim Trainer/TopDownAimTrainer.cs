using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//Game mode "Top Down Aim Trainer" main class
public class TopDownAimTrainer : MonoBehaviour
{
    private LayerMask noSpawnLayerMask;
    [SerializeField]
    private Transform minSpawnRange, maxSpawnRange;
    [SerializeField]
    private GameObject botPrefab;
    [SerializeField]
    private float spawnLifetime, spawnRate;
    private List<GameObject> botsList;
    private Vector3 spawnOffset;
    private int difficulty;

    void Start()
    {
        difficulty = PlayerPrefs.GetInt("Difficulty", 0);
        SetDifficulty();
        noSpawnLayerMask = LayerMask.GetMask("NoSpawn", "Enemy"); //Get layer mask for no spawn zone
        int maxBots = Mathf.RoundToInt(spawnLifetime/spawnRate) + 2; //Get max potential bots active simultaneously + 2 for object pooling
        botsList = new List<GameObject>();
        botPrefab.GetComponent<DisableAfterTime>().timeOut = spawnLifetime;
        spawnOffset = botPrefab.transform.position;

        for (int i = 0; i < maxBots; i++) //Create bot pool
        {
            botsList.Add(Instantiate(botPrefab, Vector3.zero, Quaternion.identity));
        }

        Invoke("spawnBot", 1); //Starts bot spawning loop, when a bot is disabled it calls for a respawn after a set amount of time
    }

    void spawnBot()
    {
        Vector3 randomPos = new Vector3(UnityEngine.Random.Range(minSpawnRange.position.x, maxSpawnRange.position.x), 0, UnityEngine.Random.Range(minSpawnRange.position.z, maxSpawnRange.position.z)) + spawnOffset;

        while (Physics.Raycast(randomPos + Vector3.up, Vector3.down, 5f, noSpawnLayerMask)) //Hit a no spawn area or another enemy
        {
            //Get new randomPos
            randomPos = new Vector3(UnityEngine.Random.Range(minSpawnRange.position.x, maxSpawnRange.position.x), 0, UnityEngine.Random.Range(minSpawnRange.position.z, maxSpawnRange.position.z)) + spawnOffset;
        }

        GameObject spawnedBot = null;
        foreach (GameObject bot in botsList)
        {
            if (bot.activeSelf == false) //Gets an inactive bot
            {
                spawnedBot = bot;
                break; 
            }
        }

        spawnedBot.transform.position = randomPos;
        StartCoroutine(ActivateBot(spawnedBot));
    }

    IEnumerator ActivateBot(GameObject bot)
    {
        yield return new WaitForEndOfFrame();
        bot.SetActive(true);
    }

    void SetDifficulty() //Change game speed according to difficulty (higher difficulty = faster spawn rates), difficulty set from game selection screen.
    {
        if (difficulty == 0)
        {
            spawnRate = 10f;
            spawnLifetime = 10f;
        } else if (difficulty == 1)
        {
            spawnRate = 5f;
            spawnLifetime = 5f;
        } else {
            spawnRate = 3f;
            spawnLifetime = 3f;
        }
    }

    void endGame()
    {
        CancelInvoke();
        Time.timeScale = 0;
    }

    void OnEnable()
    {
        GameManager.spawnEnemy += spawnBot;
        GameManager.gameEnd += endGame;
    }

    void OnDisable()
    {
        GameManager.spawnEnemy -= spawnBot;
        GameManager.gameEnd -= endGame;
    }
}
