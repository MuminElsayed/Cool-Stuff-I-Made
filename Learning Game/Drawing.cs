using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

//Color sprites with different colors depending on where you click ^^
public class Drawing : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler
{
    //Reference mess
    [SerializeField]
    private GameObject trailPrefab, allColors, sparklesPrefab;
    private Camera cam;
    private bool drawing = false;
    private GameObject currentTrail;
    private int currentColorID, trailCounter, lastColor;
    public ColorClass[] colors;
    private List<GameObject> trails = new List<GameObject>();
    [SerializeField]
    private GameObject drawings, cursor;
    private Image cursorImage;
    private Vector3 cursorPos;
    [SerializeField]
    private AudioClip coloredEffect;
    private AudioSource audioSrc;
    private ParticleSystem sparksPS;

    void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        Cursor.visible = false;
        cursorImage = cursor.GetComponentInChildren<Image>();
        audioSrc = GetComponent<AudioSource>();
        sparksPS = sparklesPrefab.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        //Sets cursor position
        cursorPos = cam.ScreenToWorldPoint(Input.mousePosition);
        cursor.transform.position = new Vector3(cursorPos.x, cursorPos.y, cursor.transform.position.z);
        //Detect mouse click
        if (Input.GetMouseButtonDown(0))
        {
            magicPaint();
        }
    }

    void magicPaint() //This is the cool part
    {
        Vector2 cursorPos2D = new Vector2(cursorPos.x, cursorPos.y); //Get position of the cursor
        RaycastHit2D[] allHits = Physics2D.RaycastAll(cursorPos + Vector3.forward, Vector2.zero, 2); //Save current hits on cursor (max 2 overlaps)
        if (allHits[0])
        {
            RaycastHit2D topHit = allHits[0];
            foreach (RaycastHit2D hit in allHits) //Checks for overlaps and sets topsmost layer, best if only 2 overlap
            {
                if (hit.collider.GetComponent<SpriteRenderer>().sortingOrder > topHit.collider.GetComponent<SpriteRenderer>().sortingOrder) //Sorting order higher than topmost hit
                {
                    topHit = hit; //Change topmost hit
                }
            }
            try
            {
                topHit.collider.GetComponent<SpriteRenderer>().color = colors[currentColorID].color; //Change color of top hit
                var main = sparksPS.main;
                main.startColor = colors[currentColorID].color;
                sparksPS.Play();
                playClip(coloredEffect);
            }
            catch (System.Exception)
            {
                print("No sprite renderer detected");
                throw;
            }
        }
    }

    public void Undo() //Deactivates the most recent trail
    {
        if (trailCounter > 0)
        {
            trails[trailCounter - 1].SetActive(false);
            trailCounter --;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) //If can draw, creates a trail
    {
        
        // print(eventData.pointerEnter.GetComponentInChildren<TextMeshProUGUI>().text); //Gets the text in the object clicked on (the letter)
        if (eventData.pointerEnter.tag == "DrawingBoard" && !drawing && currentColorID > -1) //If pointer is on drawing background (can draw)
        {
            //Creates trail and follows mouse
            drawing = true;
            currentTrail = Instantiate(trailPrefab, Vector3.zero, Quaternion.identity, drawings.transform);
            currentTrail.GetComponent<TrailRenderer>().startColor = colors[currentColorID].color;
            currentTrail.GetComponent<TrailRenderer>().endColor = colors[currentColorID].color;
            trails.Add(currentTrail);
            trailCounter = trails.Count;
        }
    }

    public void OnDrag(PointerEventData eventData) //Trail follows mouse
    {
        if (drawing)
        {
            Vector2 clickPos = cam.ScreenToWorldPoint(eventData.position); //Gets mouse position on screen
            currentTrail.transform.position = new Vector3(clickPos.x, clickPos.y, currentTrail.transform.position.z); //Trail follows the mouse
        }
    }

    public void OnEndDrag(PointerEventData eventData) //Stopped trail
    {
        drawing = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.visible = false;
    }

    public void changeColor(int colorCode) //Called from each color button
    {
        if (colorCode == -1) //Puts it on white if first click
        {
            currentColorID = -1;
            cursorImage.color = Color.white;
        } else {
            currentColorID = colorCode;
            cursorImage.color = colors[colorCode].color;
        }
    }

    private void playClip(AudioClip clip)
    {
        audioSrc.clip = clip;
        audioSrc.Play();
    }


    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
