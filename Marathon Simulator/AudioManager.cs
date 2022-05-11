using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField]
    private AudioClip[] BGM;
    private AudioSource[] audioSources; //Source 0 is for BGM, source 1 for audio clips
    private int lastBGM;
    private bool playMusic = true;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        audioSources = GetComponents<AudioSource>();

        queueBGM();
    }

    void queueBGM() //Create a music playlist
    {
        //Play music in an endless playlist
        int randomNum = UnityEngine.Random.Range(0, BGM.Length); //Can shuffle list instead
        while (randomNum == lastBGM)
        {
            randomNum = UnityEngine.Random.Range(0, BGM.Length);
        }

        PlayBGM(BGM[randomNum]);
        lastBGM = randomNum;
    }

    void PlayBGM(AudioClip clip)
    {
        audioSources[0].clip = clip;
        audioSources[0].Play();

        //Plays another song after this one finishes
        float songTime = clip.length + 2f;
        Invoke("queueBGM", songTime);
    }

    public void PlayAudio(AudioClip clip, float pitchValue)
    {
        audioSources[1].clip = clip;
        audioSources[1].pitch = pitchValue;
        audioSources[1].Play();
    }

    public void ToggleMusic(bool value)
    {
        if (value)
        {
            audioSources[0].Stop();
        } else {
            queueBGM();
        }
    }
}
