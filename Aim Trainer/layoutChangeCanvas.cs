using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;


//The manager to save, change, and set default positions of a game UI
public class layoutChangeCanvas : MonoBehaviour
{
    [SerializeField]
    private GameObject[] layoutChangeObjects;
    [SerializeField]
    private List<GameObject> activeObjects;
    [SerializeField]
    private GameObject layoutOptions;
    public static Action ResetLayoutAction, SaveLayoutAction;


    void LayoutChangeView() //Enables a mockup UI of same game UI but with different scripts for different functionality (change position, set default, get saved positions)
    {
        foreach (Transform obj in transform) //Disable real game UI
        {
            if (obj.gameObject.activeInHierarchy)
            {
                activeObjects.Add(obj.gameObject); //Saves current active UI
            }
            obj.gameObject.SetActive(false);
        }
        foreach (GameObject obj in layoutChangeObjects) //Enable the mockup UI
        {
            obj.SetActive(true);
        }

        layoutOptions.SetActive(true); //Obj with options to change UI (increase/decrease size, hide UI element)
    }

    void GameView() //Enables actual game UI
    {
        foreach (Transform obj in transform)
        {
            obj.gameObject.SetActive(false);
        }

        foreach (GameObject obj in activeObjects)
        {
            obj.SetActive(true);
        }

        layoutOptions.SetActive(false); //Disable UI editor obj
    }

    public void ResetLayoutPositions() //Calls event to reset to default positions
    {
        ResetLayoutAction();
    }

    public void SaveLayoutUI() //Calls event to save current UI positions
    {
        SaveLayoutAction();
        GameView();
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        settingsUI.changeLayoutAction += layoutChangeView;
    }

    void OnDisable()
    {
        settingsUI.changeLayoutAction -= layoutChangeView;
    }
}
