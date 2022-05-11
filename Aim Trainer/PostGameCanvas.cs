using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PostGameCanvas : MonoBehaviour
{
    public static PostGameCanvas instance;
    [SerializeField]
    private TextMeshProUGUI totalScoreText, playerStatsText, LBnames, LBranks, LBscore;\
    //Player stats
    public int playerShotsFired, playerHeadshotCount, playerBodyshotCount, playerShotsMissed, playerGameTime, playerScore;
    public float playerAccuracy {get; set;}
    [SerializeField]
    private GameObject playerStats, leaderboard, optionsMenu;
    public int leaderboardMaxPages, currentLBpage = 1;
    public List<Score> scores;
    private string playerName;
    [SerializeField]
    private Animator LBanim;
    private bool postedScore;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }
        postedScore = false;
        openOptionsMenu();
    }

    void Start()
    {
        leaderboardMaxPages = 5;
        currentLBpage = 1;
        ChangeLBPages(currentLBpage);
        scores = new List<Score>();
        LBanim.SetBool("ShowScores", false);
        playerName = PlayerPrefs.GetString("playerName", "default");
    }

    public void SetStats() //Set player stats at the end of the level, values derived from GameManager
    {
        playerStatsText.text = "Shots Fired: " + playerShotsFired.ToString() + "\n" + 
        "Headshots: " + playerHeadshotCount.ToString()  + "\n" + 
        "Bodyshots: " + playerBodyshotCount.ToString() + "\n" + 
        "Shots missed: " + playerShotsMissed.ToString() + "\n" + 
        "Accuracy: " + Mathf.Round(playerAccuracy).ToString() + "%" + "\n" + 
        "Time: " + playerGameTime.ToString() + " seconds";

        totalScoreText.text = playerScore.ToString();
    }

    public void GetPlayerStats()
    {
        playerStats.SetActive(true);
        leaderboard.SetActive(false);
        optionsMenu.SetActive(false);
    }

    public void OpenLeaderboard()
    {
        PostScore();
        playerStats.SetActive(false);
        leaderboard.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void RestartTraining()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenOptionsMenu()
    {
        playerStats.SetActive(false);
        leaderboard.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void NextLeaderboardPage()
    {
        if (currentLBpage >= leaderboardMaxPages)
        {
            currentLBpage = 1;
        } else {
            currentLBpage++;
        }
        ChangeLBPages(currentLBpage);
    }

    public void PrevLeaderboardPage()
    {
        if (currentLBpage <= 1)
        {
            currentLBpage = leaderboardMaxPages;
        } else {
            currentLBpage--;
        }
        ChangeLBPages(currentLBpage);
    }

    void ChangeLBPages(int pageNumber)
    {
        LBnames.pageToDisplay = currentLBpage;
        LBranks.pageToDisplay = currentLBpage;
        LBscore.pageToDisplay = currentLBpage;
    }

    public void GetScore() //Get scores from the database
    {
        scores = leaderboardHandler.instance.RetrieveScores();
    }

    public void UpdateLeaderboard(List<Score> updatedScores) //Update LB text
    {
        scores = updatedScores;
        int counter = 1;
        foreach (Score score in scores)
        {
            if (counter == 1)
            {
                LBranks.text = counter + ".\n";
                LBnames.text = score.name + "\n";
                LBscore.text = score.score + "\n";
            } else {
                LBranks.text += counter + ".\n";
                LBnames.text += score.name + "\n";
                LBscore.text += score.score + "\n";
            }
            counter++;
        }
        leaderboardMaxPages = Mathf.RoundToInt((float)scores.Capacity/10f);
        LBanim.SetBool("ShowScores", true);
    }

    public void PostScore() //Post score to the leaderboard handler
    {
        if (!postedScore && playerScore != 0)
        {
            leaderboardHandler.instance.PostScores(playerName, playerScore);
            GetScore();
            postedScore = true;
        } else {
            GetScore();
        }
    }

    void OnEnable()
    {
        GameManager.playerInput.PlayerMain.Disable();
        GameManager.gameEnd += PostScore;
    }

    void OnDisable()
    {
        GameManager.playerInput.PlayerMain.Enable();
        GameManager.gameEnd -= PostScore;
    }
}