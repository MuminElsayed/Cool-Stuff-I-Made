using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//A cursor that changes shape on click, and makes sparkles!
public class Cursor : MonoBehaviour, IPointerEnterHandler
{
    public static cursor instance;
    private Camera cam;
    [SerializeField]
    private Sprite[] cursorImages;
    private Image currentCursorImage;
    private ParticleSystem sparkles;
    private AudioSource sparklesSrc;

    void Awake()
    {
        cam = Camera.main;
        instance = this;
    }
    void Start()
    {
        Cursor.visible = false;
        sparkles = GetComponentInChildren<ParticleSystem>();
        currentCursorImage = GetComponentInChildren<Image>();
        sparklesSrc = GetComponent<AudioSource>();
    }    

    void Update()
    {
        //Sets cursor position
        Vector3 cursorPos = cam.ScreenToWorldPoint(Input.mousePosition);
        this.transform.position = new Vector3(cursorPos.x, cursorPos.y, this.transform.position.z);
        if (Input.GetMouseButtonDown(0))
        {
            mouseClick();
        } else if (Input.GetMouseButtonUp(0))
        {
            mouseIdle();
        }
    }
    void mouseClick()
    {
        currentCursorImage.sprite =  cursorImages[1];
    }

    void mouseIdle()
    {
        currentCursorImage.sprite =  cursorImages[0];
    }

    public void playMouseSparkles()
    {
        sparkles.Play();
        sparklesSrc.Play();
    }

    public void changeCursorColor(Color targetColor)
    {
        currentCursorImage.color = targetColor;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.visible = false;
    }
}
