using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Video;

//Basic video player, doesn't work with WebGL. (Need GIF or streamed video)
public class VideoPlayer : MonoBehaviour
{
    private UnityEngine.Video.VideoPlayer vidPlayer;
    private string videoName;
    void OnEnable()
    {
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        vidPlayer = GetComponentInChildren<UnityEngine.Video.VideoPlayer>();
        vidPlayer.url = Path.Combine(Application.streamingAssetsPath, videoName);
        vidPlayer.isLooping = true;
        vidPlayer.Prepare();
        while (!vidPlayer.isPrepared)
        {
            yield return null;
        }
        vidPlayer.Play();
    }
}
