using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//A quiz game that goes through segments of paragraphs and questions at the same time. (With audio)
//EXAMPLE: Segment 1: 2 paragraphs followed by 2 questions -> Segment 2: 3 paragraphs followed by 2 questions.
public class CreaturesQuiz : MonoBehaviour
{
    [System.Serializable]
    public class Paragraphs
    {
        [TextArea(5,10)]
        public string text;
        public AudioClip audio;
    }
    [System.Serializable]
    public class Questions    
    {
        [TextArea(1,2)]
        public string text;
        public AudioClip audio;
        [TextArea(2,3)]
        public string[] answers = new string[] {"1", "2", "3"};
        public int correctAnswer;
    }
    [System.Serializable]
    public class Segment
    {
        public Paragraphs[] paragraphs;
        public Questions[] questions;
    }

    [SerializeField]
    private GameObject introPanel, factsPanel, gamePanel;
    public Segment[] segments;
    private int currentSegment = 0;
    private int currentParagraph = 0;
    private int currentQuestion = 0;
    [SerializeField]
    private TextMeshProUGUI paragraphText, questionText;
    [SerializeField]
    private TextMeshProUGUI[] answersText;
    [SerializeField]
    private GameObject[] answerButtons;
    [SerializeField]
    private GameObject nextButton, backButton, sharkEatAnimation;
    [SerializeField]
    private Image[] answerButtonsImages;
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip correctAnswer, wrongAnswer, title;
    [SerializeField]
    private Sprite goodFoodSprite, badFoodSprite;
    private bool canInterrupt = true;
    private Image sharkAnim;

    void Start()
    {
        paragraphText.text = segments[currentSegment].paragraphs[currentParagraph].text;
        audioSource = GetComponent<AudioSource>();
        introPanel.SetActive(true);
        PlayAudioClip(title, 0.5f, false);
        sharkAnim = sharkEatAnimation.GetComponentInChildren<Image>();
    }

    public void GetParagraph()
    {
        paragraphText.text = segments[currentSegment].paragraphs[currentParagraph].text;
        //Play paragraph audio here
        PlayAudioClip(segments[currentSegment].paragraphs[currentParagraph].audio, 1f, true);
        paragraphText.gameObject.SetActive(true);
        questionText.gameObject.SetActive(false);
    }

    public void GetQuestion()
    {
        questionText.text = segments[currentSegment].questions[currentQuestion].text;
        PlayAudioClip(segments[currentSegment].questions[currentQuestion].audio, 1f, true);
        for (int i = 0; i < answersText.Length; i++)
        {
            answersText[i].text = segments[currentSegment].questions[currentQuestion].answers[i]; //Sets answersText from the segment answers
        }
        paragraphText.gameObject.SetActive(false);
        questionText.gameObject.SetActive(true);
    }

    public void SelectAnswer(int answer)
    {
        if (answer == segments[currentSegment].questions[currentQuestion].correctAnswer)
        {
            //Correct answer
            StartCoroutine(CorrectAnswer(answer));
        } else {
            //Wrong answer
            StartCoroutine(WrongAnswer(answer));
        }
    }

    private IEnumerator CorrectAnswer(int answer)
    {
        answerButtons[answer].SetActive(false);
        answerButtonsImages[answer].sprite = goodFoodSprite;
        answerButtonsImages[answer].enabled = true;
        answerButtons[answer].SetActive(false);
        sharkEatAnimation.transform.position = new Vector3 (sharkEatAnimation.transform.position.x, answerButtons[answer].transform.position.y + 50, sharkEatAnimation.transform.position.z);
        sharkEatAnimation.SetActive(true);
        yield return new WaitForSeconds(1f);
        sharkAnim.color = Color.white;
        PlayAudioClip(correctAnswer, 0f, false);
        answerButtonsImages[answer].enabled = false;
        yield return new WaitForSeconds(1f);
        answerButtons[answer].SetActive(true);
        sharkEatAnimation.SetActive(false);
        NextParagraph();
    }

    private IEnumerator WrongAnswer(int answer)
    {
        sharkEatAnimation.transform.position = new Vector3 (sharkEatAnimation.transform.position.x, answerButtons[answer].transform.position.y + 50, sharkEatAnimation.transform.position.z);
        answerButtons[answer].SetActive(false);
        answerButtonsImages[answer].sprite = badFoodSprite;
        answerButtonsImages[answer].enabled = true;
        answerButtons[answer].SetActive(false);
        sharkEatAnimation.SetActive(true);
        yield return new WaitForSeconds(1f);
        answerButtonsImages[answer].enabled = false;
        sharkAnim.color = new Color(140, 222, 0);
        PlayAudioClip(wrongAnswer, 0f, false);
        yield return new WaitForSeconds(1f);
        answerButtons[answer].SetActive(true);
        sharkEatAnimation.SetActive(false);
    }

    public void PreviousParagraph()
    {
        if (currentSegment > 0)
        {
            currentSegment --;
            currentQuestion = 0;
            GetParagraph();
        }
    }

    public void NextParagraph()
    {
        if (currentSegment < segments.Length - 1)
        {
            currentSegment ++;
            currentParagraph = 0;
            currentQuestion = 0;
            GetParagraph();
        } else {
            print("Quiz completed!");
        }
    }

    public void NextButton() //Goes to next question or paragraph panel, depending on current active one
    {
        if (paragraphText.gameObject.activeInHierarchy && currentParagraph == segments[currentSegment].paragraphs.Length - 1) //Paragraph is on the screen, and finished all paragraphs of the segment
        {
            GetQuestion();
        } else if (questionText.gameObject.activeInHierarchy && currentQuestion == segments[currentSegment].questions.Length - 1) //The last question is on the screen
        {
            if (currentSegment == segments.Length - 1) //Last segment
            {
                GoToFacts();
            }
            NextParagraph();
        } else if (currentParagraph < segments[currentSegment].paragraphs.Length - 1){ //Paragraphs remaining in the segment
            currentParagraph ++;
            GetParagraph();
        } else if (currentQuestion < segments[currentSegment].questions.Length - 1) //Questions remaining in paragraph
        {
            currentQuestion ++;
            GetQuestion();
        }
        backButton.SetActive(true);
    }

    public void BackButton() //Goes to previous question or paragraph panel, depending on current active one
    {
        if (paragraphText.gameObject.activeInHierarchy && currentParagraph == 0) //Paragraph is on the screen
        {
            PreviousParagraph();
        } else if (questionText.gameObject.activeInHierarchy) //Question is on the screen
        {
            GetParagraph();
        } else if (currentQuestion > 0)
        {
            currentQuestion --;
            GetQuestion();
        } else if (currentParagraph > 0){
            currentParagraph --;
            GetParagraph();
        }
        if (currentSegment == 0 && currentParagraph == 0 && currentQuestion == 0)
        {
            backButton.SetActive(false);
        }
        NextButton.SetActive(true);
    }

    private void PlayAudioClip(AudioClip audioClip, float delay, bool interrupt)
    {
        StartCoroutine(PlayClip(audioClip, delay, interrupt));
    }

    IEnumerator PlayClip(AudioClip audioClip, float delay, bool interruptStatus) //Before the invention of an AudioManager
    {
        if (!canInterrupt)
        {
            while (audioSource.isPlaying)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(delay);
            audioSource.clip = audioClip;
            canInterrupt = interruptStatus;
            audioSource.Play();
        } else {
            audioSource.clip = audioClip;
            audioSource.Play();
            canInterrupt = interruptStatus;
        }
    }

    public void StartQuiz()
    {
        gamePanel.SetActive(true);
        introPanel.SetActive(false);
        factsPanel.SetActive(false);
        GetParagraph();
    }

    void GoToFacts()
    {
        factsPanel.SetActive(true);
        gamePanel.SetActive(false);
    }
}
