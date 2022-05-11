using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Import sprites from an array and creates them as object in the scene. Used as bulk import/add to scene
[ExecuteInEditMode]
public class SpriteImporter : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    void OnEnable()
    {
        int counter = 0;
        foreach (Sprite sprite in sprites)
        {
            GameObject obj = new GameObject("Sprite " + counter);
            obj.AddComponent<SpriteRenderer>().sprite = sprites[counter];
            obj.transform.parent = transform;
            // Instantiate(obj, transform);
            counter ++;
        }
    }
}
