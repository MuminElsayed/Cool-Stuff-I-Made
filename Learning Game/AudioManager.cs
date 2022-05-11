using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField]
    private AudioClip[] correctAnswers, wrongAnswers;
    private AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }
        audioSource = GetComponent<AudioSource>();
    }

    public void playAudio(AudioClip[] clips, float delay, float startingDelay)
    {
        StartCoroutine(playAudioClips(clips, delay, startingDelay));
    }

    public void playAudio(AudioClip clip)
    {
        AudioClip[] tempArray = new AudioClip[] {clip};
        StartCoroutine(playAudioClips(tempArray, 0, 0));
    }
    public void playAudio(AudioClip clip, float delay)
    {
        AudioClip[] tempArray = new AudioClip[] {clip};
        StartCoroutine(playAudioClips(tempArray, 0, delay));
    }

    public void playCorrectAnswer()
    {
        playAudio(correctAnswers[UnityEngine.Random.Range(0, correctAnswers.Length)]);
        cursor.instance.playMouseSparkles();
    }

    public void playWrongAnswer()
    {
        playAudio(wrongAnswers[UnityEngine.Random.Range(0, wrongAnswers.Length)]);
    }

    private IEnumerator playAudioClips(AudioClip[] clips, float delay, float startingDelay)
    {
        yield return new WaitForSeconds(startingDelay);
        foreach (AudioClip clip in clips)
        {
            audioSource.clip = clip;
            audioSource.Play();
            yield return new WaitForSeconds(delay);
        }
    }
}
