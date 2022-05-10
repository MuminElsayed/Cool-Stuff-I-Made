using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the class that contains the options to control UI elements
public class layoutOptions : MonoBehaviour
{
    private RectTransform rect, targetRect;

    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void SetParent(RectTransform parent) //Moves the UI change options to the last touched UI element
    {
        targetRect = parent;
        transform.SetParent(parent);
        rect.anchoredPosition = Vector3.zero;
        rect.localScale = 1/targetRect.localScale.x * 2f * Vector3.one;
    }

    public void HideTarget() //Hide UI element
    {
        transform.SetParent(transform.root);
        rect.localScale = Vector3.zero;
        targetRect.gameObject.SetActive(false);
    }

    public void SizeIncrease() //Increase scale of UI element
    {
        if (targetRect.localScale.x < 2)
        {
            targetRect.localScale += Vector3.one * 0.1f;
        }
        rect.localScale = 1/targetRect.localScale.x * 2f * Vector3.one;
    }

    public void SizeDecrease() //Decrease scale of UI element
    {
        if (targetRect.localScale.x > 0.5f)
        {
            targetRect.localScale -= Vector3.one * 0.1f;
        }
        rect.localScale = 1/targetRect.localScale.x * 2f * Vector3.one;
    }
    
    void OnEnable()
    {
        layoutChanger.gettingDragged += SetParent;
    }

    void OnDisable()
    {
        layoutChanger.gettingDragged -= SetParent;
    }
}
