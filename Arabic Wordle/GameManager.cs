using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTT.MiniGame.WordFinder;
using DTT.MinigameBase;
using TMPro;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Game system references
    [SerializeField]
    private WordFinderManager _wordFinderManager;
    [SerializeField]
    private WordFinderLevel _level;
    [SerializeField]
    private WordFinderConfig _config;
    [SerializeField]
    private TextAsset _wordListFile;
    private List<string> _wordList, possibleWords;
    
    //Level Progression variables
    private int gridSize, timerBonus, playerScore, highscore, difficultyLevel;
    public int currentLevel;
    public bool endlessMode { get; set;}
    
    //UI References
    [SerializeField]
    private GameObject gameOverPanel, notEnoughStarsPanel, levelsHolder;
    [SerializeField]
    private Image[] starsHolder;
    [SerializeField]
    private Sprite starSprite, emptyStarSprite;
    private LevelObject[] allLevels;
    [SerializeField]
    private TextMeshProUGUI starsText, scoreText, highscoreText;

    // playerScore = sum of remaining time at the end of each level
    // Each level is repeated 5 times.
    // Levels 0-3: difficultyLevel: 1; grid size 3x3, 4x4, 5x5; timerBonus: 3 seconds.
    // LeveLs 4-7: difficultyLevel: 2; grid size 6x6, 7x7, 8x8; timerBonus: 5 seconds.
    // Levels 8-10: difficultyLevel: 3; grid size 9x9, 10x10, 11x11; timerBonus: 7 seconds.

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public GameManager()
    {
        currentLevel = 1;
        playerScore = 0;
    }

    void Start()
    {
        //Import word list from .txt file
        var content = _wordListFile.text;
        //Clean up text file and add to a list
        var AllWords = content.Split(new char[] { '\n', '\r', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
        _wordList = new List<string>(AllWords);
        //Setup level UI
        highscore = PlayerPrefs.GetInt("Highscore", 0);
        highscoreText.text = "Highscore: " + highscore;
        allLevels = levelsHolder.GetComponentsInChildren<LevelObject>();
        UpdateStarsText();
    }

    public void StartGameEndless()
    {
        AddScore(0);
        ChangeDifficulty(currentLevel);
        NewLevel(31f);
    }

    public void StartGameClassic(float startTime, int levelNum)
    {
        ResetGame();
        NewLevel(startTime);
        currentLevel = levelNum;
    }

    void ChangeDifficulty(int currentLevel) //Difficulty level scales with current level (level 1-3: easy; level 4-7: medium; level 8+: hard)
    {
        //Change current difficultyLevel according to level
        if (currentLevel <= 3)
        {
            difficultyLevel = 1;
            timerBonus = 3;
        } else if (currentLevel <= 7) {
            difficultyLevel = 2;
            timerBonus = 5;
        } else {
            difficultyLevel = 3;
            timerBonus = 7;
        }
        //Set changes relative to difficultyLevel
        int wordCount;
        if (currentLevel > 9)
        {
            gridSize = 11; //Min 3x3, max 11x11
            wordCount = 11; //Min 3, max 11
        } else {
            gridSize = currentLevel + 2; //Min 3x3, max 11x11
            wordCount = currentLevel + 2; //Min 3, max 11
        }
        ChangeGrid(gridSize, wordCount);
    }

    public void ChangeGrid(int gridSize, int wordCount)
    {
        _config.gridSize = Vector2Int.one * gridSize;
        _config.wordCount = wordCount;
    }

    void NewLevel(float startTime)
    {
        if (currentLevel != 1) //If not first level, continue timer
        {
            _config.maxTime = _wordFinderManager.PlayTime;
        } else { //First level
            _config.maxTime = startTime;
        }
        //Set level config
        _config.possibleWords = _wordList;
        _level.ChangeLevelSettings(_config);
        //Start new level
        _wordFinderManager.StartGame(_level);
    }

    public void AddScore(int score)
    {
        playerScore += score;
        if (playerScore < 0)
        {
            playerScore = 0;
        }
        //Update highscore
        if (playerScore > highscore)
        {
            highscore = playerScore;
            highscoreText.text = "Highscore: " + highscore;
            //Save highscore
            PlayerPrefs.SetInt("Highscore", highscore);
        }
        scoreText.text = "Score: " + playerScore;
    }

    public void AddTime(float time)
    {
        _wordFinderManager.AddTime(time);
    }


    public void CorrectWord(bool value, string word)
    {
        if (value) //Correct word
        {
            AddScore(difficultyLevel * timerBonus);
            AddTime(timerBonus);
        } else { //Wrong word
            AddScore(-difficultyLevel * timerBonus);
            AddTime(-timerBonus);
        }
    }

    public void LevelComplete(WordFinderResult result)
    {
        //Prints out the result of the level
        // Debug.Log(result.ToString());
        if (endlessMode) //If endless mode, go to next level
        {
            currentLevel ++;
            ChangeDifficulty(currentLevel);
            NewLevel(0);
        } else { //Classic Mode: finish the level
            _wordFinderManager.ForceFinish();
            //Get completion %
            float scorePercentage = (_config.maxTime - result.TimeTaken) / _config.maxTime;
            // Debug.Log("Score: " + scorePercentage + "%");
            int starsEarned; //Calculate amount of stars earned according to completion %
            if (scorePercentage > 0.6f) {
                //Three stars if score > 60%
                starsEarned = 3;
            } else if (scorePercentage > 0.4f) {
                //2 stars if score > 40%
                starsEarned = 2;
            } else {
                //1 star
                starsEarned = 1;
            }
            SetStars(starsEarned);
            AddStarsEarned(starsEarned, currentLevel);
            ShowAd();
        }
    }

    public void NextLevel() //Go to next level in the classic mode
    {
        int currentStars = PlayerPrefs.GetInt("PlayerStars", 0);
        if (currentLevel + 1 < allLevels.Length && currentStars >= allLevels[currentLevel + 1].starsRequired) //If there exists a next level & player meets minimum stars requirement
        {
            LevelObject nextLevel = allLevels[currentLevel + 1];
            ResetGame(); //Reset the game
            //Start a new game with next level's settings
            ChangeGrid(nextLevel.gridSize, nextLevel.gridSize);
            StartGameClassic(nextLevel.levelTime, nextLevel.levelNum);
        } else {
            notEnoughStarsPanel.SetActive(true);
        }
    }

    public void GameOver(WordFinderResult result)
    {
        gameOverPanel.SetActive(true);
        SetStars(0);
        ShowAd();
    }

    void AddStarsEarned(int starsEarned, int levelNum)
    {
        int newStarsEarned = starsEarned - allLevels[levelNum].StarsEarned; //Actual stars earned (if player replayed the level and earned new stars they number won't overlap)
        // print("New Stars Earned: " + newStarsEarned);
        PlayerPrefs.SetInt("PlayerStars", PlayerPrefs.GetInt("PlayerStars", 0) + newStarsEarned); //Adds new stars to current earned stars
        allLevels[levelNum].StarsEarned = newStarsEarned + allLevels[levelNum].StarsEarned; //Saves new number of stars earned
        UpdateStarsText();
    }

    public void UpdateStarsText()
    {
        starsText.text = PlayerPrefs.GetInt("PlayerStars", 0).ToString();
    }

    public void RestartLevel()
    {
        StartGameClassic(allLevels[currentLevel].levelTime, currentLevel);
    }

    void ResetGame()
    {
        currentLevel = 1;
        playerScore = 0;
        highscore = 0;
        highscoreText.text = "Highscore: " + highscore;
        PlayerPrefs.SetInt("Highscore", highscore);
        gameOverPanel.SetActive(false);
        _wordFinderManager.ClearGame();
    }

    void SetStars(int numberOfStars) //Min 1 star, max 3 stars
    {
        for (int i = 0; i < numberOfStars; i++) //Set full stars
        {
            starsHolder[i].sprite = starSprite;
        }
        int[] array = {0, 1};
        for (int i = 0; i < 3 - numberOfStars; i++) //Set empty stars
        {
            starsHolder[i + numberOfStars].sprite = emptyStarSprite;
        }
        // print ("Stars Earned: " + numberOfStars);
    }

    void ShowAd()
    {
        if (allLevels[currentLevel].stageNum > 1)
            AdManager.instance.ShowInterstatialAd();
    }

    void OnEnable()
    {
        //Subscribe to events
        _wordFinderManager.NextLevel += LevelComplete;
        _wordFinderManager.Finish += GameOver;
        if (endlessMode)
            _wordFinderManager.EventHandler.OnWordCompleted += CorrectWord;
    }

    void OnDisable()
    {
        //Unsubscribe from events
        _wordFinderManager.NextLevel -= LevelComplete;
        _wordFinderManager.Finish -= GameOver;
        if (endlessMode)
            _wordFinderManager.EventHandler.OnWordCompleted -= CorrectWord;
    }

    //Leet code answers
    public string RemoveDuplicates(string s, int k) 
    {
        string result = null;
        Stack stack = new Stack();
        char lastLetter = ' ';
        int counter = 0;
        for (int i = 0; i < s.Length; i++)
        {
            if (lastLetter == s[i]) //Check if duplicate value
            {
                counter ++; //Count how many duplicates in a row
            } else {
                counter = 0; //Reset counter if not reached
            }
            if (counter == k - 1) //Reached deletion limit
            {
                PopStacks(stack, k); //Loop delete k times
            }
            stack.Push(s[i]);
        }
        
        return result;
    }

    void PopStacks(Stack targetStack, int count)
    {
        for (int i = 0; i < count; i++)
        {
            targetStack.Pop();
        }
    }
}
