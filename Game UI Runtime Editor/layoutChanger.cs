using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class layoutChanger : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    public Vector3 changedRectPos, defaultPos;
    private bool changingLayout;
    public float scale {get; set;}
    public static Action<RectTransform> GettingDragged;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        scale = 1;
    }

    public void OnPointerDown(PointerEventData pointer)
    {
        //Send action
        if (GettingDragged != null)
        {
            GettingDragged(rectTransform);
        }
    }

    public void OnEndDrag(PointerEventData pointer) //Save new position
    {
        if (changingLayout)
        {
            changedRectPos = rectTransform.position;
        }
    }

    public void OnDrag(PointerEventData eventData) //Move UI element if currently changing the UI
    {
        if (changingLayout)
        {
            rectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;
        }
    }

    public void ResetToDefault() //Reset UI to default settings
    {
        rectTransform.position = defaultPos;
        rectTransform.localScale = Vector3.one;
        scale = 1;
        changedRectPos = Vector3.zero;
        SaveLayout();
        changingLayout = true;
    }

    public void SaveLayout()
    {
        changingLayout = false;
        scale = rectTransform.localScale.x;
        //Save player data
        string jsonData = JsonUtility.ToJson(this, false);
        PlayerPrefs.SetString(gameObject.name, jsonData);
        PlayerPrefs.Save();
    }

    public void ChangeLayout()
    {
        changingLayout = true;
    }

    void OnEnable()
    {
        //Load player data from JSON file saved in player prefs
        JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(gameObject.name), this);

        if (changedRectPos != Vector3.zero) //If default value is changed
        {
            rectTransform.position = changedRectPos; //Save change in position
        } else {
            //Saves default pos
            defaultPos = rectTransform.position;
        }
        rectTransform.localScale = new Vector3(scale, scale, 1);

        //Subscribe to events
        layoutChangeCanvas.resetLayoutAction += ResetToDefault;
        layoutChangeCanvas.SaveLayoutAction += SaveLayout;
        settingsUI.ChangeLayoutAction += ChangeLayout;
    }

    void OnDisable()
    {
        //Unsubscribe to events
        layoutChangeCanvas.resetLayoutAction -= ResetToDefault;
        layoutChangeCanvas.SaveLayoutAction -= SaveLayout;
        settingsUI.ChangeLayoutAction -= ChangeLayout;
    }
}
