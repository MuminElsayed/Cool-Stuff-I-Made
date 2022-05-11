using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

//From a drag and drop game, when you release the obj you're dragging you submit it as an answer, also snaps the dragged object to the "dropped space"
public class DropSpace : MonoBehaviour, IDropHandler
{
    private RectTransform rectTransform;
    public int questionNumber;
    private int selectedAnswer;
    public static Action<bool> submitAnswer;
    private GameObject lastDragged;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void CheckAnswer()
    {
        if (questionNumber == selectedAnswer)
        {
            //correct answer
            submitAnswer(true);
            AudioManager.instance.playCorrectAnswer();
            StartCoroutine(DisableRaycast(lastDragged.GetComponent<Image>()));
        } else {
            //wrong answer
            submitAnswer(false);
            AudioManager.instance.playWrongAnswer();
        }
    }

    private IEnumerator DisableRaycast(Image image)
    {
        yield return new WaitForEndOfFrame();
        image.raycastTarget = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            // print("dropped");
            eventData.pointerDrag.GetComponent<RectTransform>().position = rectTransform.position; //Snap dropped object to center of dropped space
            lastDragged = eventData.pointerDrag;
            CheckAnswer();
        } else {
            print("pointer drag null");
        }
    }

    void ChangeSelectedAnswer(int answer)
    {
        selectedAnswer = answer;
    }

    void OnEnable()
    {
        DragNDrop.SetAnswer += ChangeSelectedAnswer;
    }

    void OnDisable() 
    {
        DragNDrop.SetAnswer += ChangeSelectedAnswer;
        StopAllCoroutines();
    }
}
