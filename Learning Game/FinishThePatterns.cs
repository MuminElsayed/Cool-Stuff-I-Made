using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

[System.Serializable]
public class PlaceHolder
{
    public GameObject[] gameObjects;
}

public class FinishThePatterns : MonoBehaviour
{
    [SerializeField]
    private int quizID, playerScore, maxScore, correctAnswers;
    [SerializeField]
    private AudioClip[] startingAudio;
    private int[][] jaggedArray = new int [5][];
    [SerializeField]
    private GameObject[] shapeHolders;
    [SerializeField]
    private Sprite[] allShapes;
    [SerializeField]
    private PlaceHolder[] placeholders;
    [SerializeField]
    private int[] randomNumbersArray;
    public static Action resetSheet;

    void Start()
    {
        NewGame();
    }

    void NewGame()
    {
        //Initialize patterns
        jaggedArray[0] = new int[] {1, 2, 1, 2, 1};
        jaggedArray[1] = new int[] {3, 3, 4, 3, 3};
        jaggedArray[2] = new int[] {0, 5, 1, 0, 5};

        //Create a random array for shape randomization
        List<int> numbersList = new List<int>{};
        for (int i = 0; i < allShapes.Length; i++)
        {
            numbersList.Add(i);
        }
        
        //Randomize the array
        var rnd = new System.Random();
        var randomNumbers = numbersList.OrderBy(item => rnd.Next());
        randomNumbersArray = randomNumbers.ToArray();
        
        for (int i = 0; i < shapeHolders.Length; i++)
        {
            shapeHolders[i].GetComponent<Image>().sprite = allShapes[i];
            shapeHolders[i].GetComponent<DragNDrop>().answerNumber = i;
        }

        //Set patterns
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < jaggedArray[i].Length; j++)
            {
                if (j == jaggedArray[i].Length - 1) //last in pattern
                {
                    placeholders[i].gameObjects[j].GetComponent<DropSpace>().questionNumber = randomNumbersArray[jaggedArray[i][j]];
                } else {
                    placeholders[i].gameObjects[j].GetComponent<Image>().sprite = allShapes[randomNumbersArray[jaggedArray[i][j]]];
                }
            }
        }
    }

    void SubmitAnswer(bool answer)
    {
        if (answer)
        {
            playerScore ++;
            correctAnswers ++;
            if (correctAnswers == maxScore)
            {
                WorksheetManager.instance.nextSheet();
            }
        } else if (playerScore > 0)
        {
            playerScore --;
        }
    }

    void OnEnable() {
        DropSpace.submitAnswer += SubmitAnswer;
        AudioManager.instance.playAudio(startingAudio, 0.75f, 1.5f);
        if (WorksheetManager.instance != null)
        {
            WorksheetManager.instance.setMaxScore(quizID, maxScore);
        }
    }

    public void resetShapes()
    {
        resetSheet();
        correctAnswers = 0;
    }

    void OnDisable()
    {
        DropSpace.submitAnswer -= SubmitAnswer;
        if (WorksheetManager.instance != null)
        {
            WorksheetManager.instance.postScore(quizID, playerScore);
        }
    }
}
