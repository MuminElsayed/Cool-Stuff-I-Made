using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
using TMPro;

//Level object holds info about each level, as well as player progression.
public class LevelObject : MonoBehaviour
{
    public int levelNum, stageNum, levelTime, starsRequired, playerStars, gridSize;
    private int starsEarned;
    public int StarsEarned
    {
        get { return GetStars(); }
        set { 
                if (value >= 3) //Max value = 3
                {
                    SaveStars(3);
                } else {
                    SaveStars(value);
                }
            }
    }
    [SerializeField]
    private Sprite lockedSprite, unlockedSprite, starSprite, emptyStarSprite;
    [SerializeField]
    private RTLTextMeshPro text;
    [SerializeField]
    private GameObject starsHolder;
    [SerializeField]
    private GameObject notEnoughStarsObj;

    void Start()
    {
        levelNum = transform.GetSiblingIndex() + 9 * (stageNum - 1); //Sets level number to it's order in the hierarchy and stage
        text.text = (levelNum + 1).ToString(); //Sets the text to the level number (+1 to start from level 1 visually)
        gridSize = stageNum + 2;
        starsRequired = 15 * (stageNum - 1); //Number of stars required for each stage (currently 15 * stageNumber)
        levelTime = gridSize * 10; //5 seconds for each word (gridSize = wordCount)
        //Set UI to show amount of stars previously earned on level
        GetStars();
        UpdateStarsUI();
        CheckIfPlayable();
    }

    public void StartGame()
    {
        GameManager.instance.ChangeGrid(gridSize, gridSize); //Changes the grid size
        GameManager.instance.StartGameClassic(levelTime, levelNum); //Starts the game with level settings
    }

    private void SaveStars(int numOfStars) //Saves stars earned for this level to Prefs
    {
        starsEarned = numOfStars;
        PlayerPrefs.SetInt("Level" + levelNum, starsEarned);
        // print("Level " + levelNum + "Saved Stars: " + starsEarned);
        UpdateStarsUI();
    }

    private int GetStars() //Gets stars earned for this level from Prefs
    {
        starsEarned = PlayerPrefs.GetInt("Level" + levelNum, 0);
        // print("Level " + levelNum + " Stars Got: " + starsEarned);
        return starsEarned;
    }

    private void UpdateStarsUI() //Update level holder to show stars earned
    {
        Image[] starsEarnedUI = starsHolder.GetComponentsInChildren<Image>();
        for (int i = 0; i < starsEarned; i++)
        {
            starsEarnedUI[i].sprite = starSprite;
        }
        for (int i = 0; i < 3 - starsEarned; i++)
        {
            starsEarnedUI[i + starsEarned].sprite = emptyStarSprite;
        }
    }

    private void HideStarsUI() //Hide stars UI if level not unlocked
    {
        foreach (var starUI in starsHolder.GetComponentsInChildren<Image>())
        {
            starUI.color = new Vector4();
        }
    }

    private void ShowStarsUI() //Show stars UI if level not unlocked
    {
        foreach (var starUI in starsHolder.GetComponentsInChildren<Image>())
        {
            starUI.color = Color.white;
        }
    }

    private void CheckIfPlayable()
    {
        if (starsRequired <= playerStars) //If the player has earned enough stars to unlock this level
        {
            //Unlock the level
            GetComponent<Image>().sprite = unlockedSprite;
            text.color = Color.black;
            GetComponent<Button>().interactable = true;
            ShowStarsUI();
        } else { //If the player has not earned enough stars to unlock this level
            //Lock the level
            GetComponent<Image>().sprite = lockedSprite;
            text.alpha = 0f;
            GetComponent<Button>().interactable = false;
            HideStarsUI();
        }
    }

    void OnEnable()
    {
        playerStars = PlayerPrefs.GetInt("PlayerStars", 0); //Gets the number of stars the player has earned
        starsEarned = GetStars(); //Gets the number of stars earned on this level
        UpdateStarsUI();
        CheckIfPlayable();
    }

    void OnDisable()
    {
        //Save the number of stars earned on this level
        SaveStars(starsEarned);
    }
}
