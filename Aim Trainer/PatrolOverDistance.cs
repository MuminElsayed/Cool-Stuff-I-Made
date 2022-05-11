using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolOverDistance : MonoBehaviour
{
    [SerializeField]
    private Vector3 direction;
    [SerializeField]
    private float speed, distance;
    private Vector3 startPos;

    void Awake()
    {
        startPos = transform.position;
    }
    void Update() //Moves the gameObject back and forth in a patrol-like motion, scaled by speed and distance
    {
        transform.position = startPos + direction * Mathf.Sin(Time.time * speed) * distance;
    }
}
