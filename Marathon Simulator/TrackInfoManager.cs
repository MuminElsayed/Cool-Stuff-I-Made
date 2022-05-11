using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This class is used to manage track information. References are set for each map, and sends info to game manager before game starts.
public class TrackInfoManager : MonoBehaviour
{
    public static trackInfoManager instance;
    [SerializeField]
    private int segmentLength;
    [SerializeField]
    private GameObject[] trackSegments, cars, carEaters, allBots;
    public static Action<GameObject[], int, GameObject[], GameObject[], GameObject[]> sendTrackInfo;

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
        //Sends saved track info to the game manager when the scene starts
        if (sendTrackInfo != null)
        {
            sendTrackInfo(trackSegments, segmentLength, cars, allBots, carEaters);
        }
    }
}
