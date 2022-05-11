using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

//Dragging part of the "Drag and drop" game, attached to every answer, on game start each draggable object has an answer number assigned to it relevant to the text/image it has.
public class DragNDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    [SerializeField]
    private Vector3 defaultPos;
    private Image image;
    private Color imageColor;
    private float imageAlpha;
    public int answerNumber;
    public static Action<int> SetAnswer; //Set the current dragged obj as the answer to submit
    public AudioClip shapeClip;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();
        imageColor = image.color;
        imageAlpha = image.color.a;
        defaultPos = rectTransform.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AudioManager.instance.playAudio(shapeClip);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        imageColor.a = 0.75f;
        image.color = imageColor;
        SetAnswer(answerNumber);
        //Add shape audio here
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        imageColor.a = imageAlpha;
        image.color = imageColor;
        //Submit answer, called on "DropSpace"
    }

    public void ResetPos()
    {
        image.raycastTarget = true;
        rectTransform.anchoredPosition = defaultPos;
    }

    void OnEnable() {
        // ResetPos();
        matchShapes.ResetAnswers += ResetPos;
        finishThePatterns.ResetSheet += ResetPos;
    }

    private void OnDisable() {
        matchShapes.ResetAnswers -= ResetPos;
        finishThePatterns.ResetSheet += ResetPos;
    }
}