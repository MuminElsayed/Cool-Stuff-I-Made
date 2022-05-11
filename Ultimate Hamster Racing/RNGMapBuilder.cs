using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Builds a random map from a list of segments. I'm still learning procedural generation ok.
public class RNGMapBuilder : MonoBehaviour
{
    [SerializeField]
    private GameObject[] obstacles;
    [SerializeField]
    private Transform startingPos;
    [SerializeField]
    private int numberOfObstacles;

    public void GenerateMap()
    {
        Vector3 startPos = startingPos.position;
        Vector3 forwardDir = Vector3.zero;
        for (int i = 0; i < numberOfObstacles; i++)
        {
            GameObject obj = Instantiate(obstacles[UnityEngine.Random.Range(0, obstacles.Length)], this.transform); //Instantiates a random obstacle
            obj.transform.position = startPos; //Sets the new obstacle's pos (first iteration is default pos, following iterations is the previous obstacles' endPosition)
            obj.transform.forward = forwardDir;
            obj.SetActive(true);
            
            Transform objTransform = obj.GetComponent<ObstacleClass>().endPos;
            startPos = objTransform.position; //Saves the next startingPos as the previous endPos
            forwardDir = objTransform.forward; //Saves the forwardDir
        }
    }
}
