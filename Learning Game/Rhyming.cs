using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


[System.Serializable]
public class Question
{
    public string Name;
    public List<string> answers;
    public Sprite sprite;
    public AudioClip audioClip;
}

public class Rhyming : MonoBehaviour
{
    [SerializeField]
    private bool PictureQuestions = true;
    [SerializeField]
    private GameObject questionObject;
    [SerializeField]
    private GameObject[] answerOptions;
    [SerializeField]
    private Question[] questions;
    private List<int> oldQuestions;
    public List<string> allAnswerWords, addedAnswerWords;
    public int questionNumber;
    public List<int> randomNums;
    private int correctAnswersCount, correctQuestionsAnswered, NumOfCorrectAnswers;
    public static Func<string, bool> SubmitAnswerAction;
    public static Action ResetButtons;


    void Start()
    {
        startGame();
    }

    void startGame()
    {
        oldQuestions = new List<int>();
        NewQuestion();
    }

    void NewQuestion()
    {
        ResetButtons();
        correctAnswersCount = 0;
        
        //Get a unique question
        questionNumber = UnityEngine.Random.Range(0, questions.Length);
        while (oldQuestions.Contains(questionNumber))
        {
            questionNumber = UnityEngine.Random.Range(0, questions.Length);
        }
        
        if (PictureQuestions)
        {
            //Set question sprite
            questionObject.GetComponent<Image>().sprite = questions[questionNumber].sprite;
        } else {
            //Set question text here
            questionObject.GetComponentInChildren<TextMeshProUGUI>().text = questions[questionNumber].Name;
        }
       
        //Set the answers
        //Put all the question words in one list
        allAnswerWords = new List<string>();
        for (int i = 0; i < questions.Length; i++)
        {
            if (i != questionNumber)
            {
                foreach (string item in questions[i].answers)
                {
                    allAnswerWords.Add(item);
                }
            }
        }
        
        //Set random words as answers
        //Create a number list of all words
        randomNums = new List<int>();
        for (int i = 0; i < allAnswerWords.Count; i++)
        {
            randomNums.Add(i);
        }

        //Set random answers
        //Pick random number and remove from list
        for (int i = 0; i < answerOptions.Length; i++)
        {
            int randomNum = randomNums[UnityEngine.Random.Range(0, randomNums.Count)];
            randomNums.Remove(randomNum);
            //Set picked num as index for word answer
            answerOptions[i].GetComponent<ButtonListener>().changeText(allAnswerWords[randomNum]);
        }

        //Set 3 correct answers
        //Get random correct answers from question class
        randomNums = new List<int>();
        List<int> randomAnswers = new List<int>();
        for (int i = 0; i < questions[questionNumber].answers.Count; i++)
        {
            randomNums.Add(i);
        }
        for (int i = 0; i < answerOptions.Length; i++)
        {
            randomAnswers.Add(i);
        }
        if (PictureQuestions)
        {
            NumOfCorrectAnswers = 3;
        } else {
            NumOfCorrectAnswers = 2;
        }

        for (int i = 0; i < NumOfCorrectAnswers; i++)
        {
            int randomNum2 = randomNums[UnityEngine.Random.Range(0, randomNums.Count)];
            randomNums.Remove(randomNum2);
            //Set random answer
            int randomAnswerNum = randomAnswers[UnityEngine.Random.Range(0, randomAnswers.Count)];
            randomAnswers.Remove(randomAnswerNum);
            answerOptions[randomAnswerNum].GetComponent<ButtonListener>().changeText(questions[questionNumber].answers[randomNum2]);
        }
    }
    public bool SubmitAnswer(string answer)
    {
        bool result;
        if (questions[questionNumber].answers.Contains(answer))
        {
            //Add question in old and get a new question
            AudioManager.instance.playCorrectAnswer();
            oldQuestions.Add(questionNumber); //Add to list to prevent duplicate questions
            correctAnswersCount++;
            result = true;
        } else {
            //Replay question audio
            AudioManager.instance.playWrongAnswer();
            result = false;
        }

        if (correctAnswersCount == NumOfCorrectAnswers)
        {
            WorksheetManager.instance.restartSheet();
            StartCoroutine(GetNewQuestion(1f));
            correctQuestionsAnswered ++;
        }

        if (correctQuestionsAnswered == 3)
        {
            WorksheetManager.instance.nextSheet();
        }

        return result;
    }

    IEnumerator GetNewQuestion(float delay)
    {
        yield return new WaitForSeconds(delay);
        NewQuestion();
    }

    void OnEnable()
    {
        Rhyming.SubmitAnswerAction += SubmitAnswer;
    }

    void OnDisable()
    {
        Rhyming.SubmitAnswerAction -= SubmitAnswer;
    }
}