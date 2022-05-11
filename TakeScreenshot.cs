using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//A simple script that takes a screenshot when you enable "takeScreenshot", and print screenshot location to console.
public class TakeScreenshot : MonoBehaviour
{
    [SerializeField]
    private string path;
    public bool takeScreenshot;
    

    void Update()
    {
        if (takeScreenshot == true)
        {
            screenshot();
        }
    }
    public void screenshot()
    {
        ScreenCapture.CaptureScreenshot(path + SceneManager.GetActiveScene().name + Time.time + ".png");
        takeScreenshot = false;
        print("Screenshot taken at " + path);
    }
}
