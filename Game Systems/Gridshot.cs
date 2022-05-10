using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//A game mode to practice aim for mobile (PC mode reference: https://www.youtube.com/watch?v=R0m4pNO10UQ)
//This is the main script for controlling spawn/placements
public class Gridshot : MonoBehaviour
{
    [SerializeField]
    private GameObject[] targets; //Provides a set of 20 targets in fixed places that 
    private GameObject lastSpawn;

    void Start()
    {
        //Puts three random objects in place on game start
        PutRandomTarget();
        PutRandomTarget();
        PutRandomTarget();
    }

    void PutRandomTarget()
    {
        //Gets unique target then spawns it
        int randomSpawn = UnityEngine.Random.Range(0, targets.Length);
        if (!targets[randomSpawn].activeInHierarchy && targets[randomSpawn] != lastSpawn) //Inactive
        {
            StartCoroutine(ActivateObj(targets[randomSpawn]));         
        } else {
            PutRandomTarget();
        }
    }

    IEnumerator ActivateObj(GameObject obj) //Spawn a unique target from a set of 20
    {
        yield return new WaitForEndOfFrame(); //Wait at the end of frame to duplicate duplicate calls.
        if (!obj.activeInHierarchy)
        {
            obj.SetActive(true);
        } else {
            PutRandomTarget();
        }
    }

    void GetLastDisabled(GameObject obj)
    {
        lastSpawn = obj;
    }

    void OnEnable()
    {
        GameManager.spawnEnemy += PutRandomTarget;
        EnemyCapsule.lastDisabled += GetLastDisabled; //Saves last disabled obj to prevent 
    }

    void OnDisable()
    {
        GameManager.spawnEnemy -= PutRandomTarget;
        EnemyCapsule.lastDisabled -= GetLastDisabled;
    }
}
