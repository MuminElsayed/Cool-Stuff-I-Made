using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[System.Serializable]
public class MapClass
{
    public string MapName;
    public UnityEngine.Object mapScene;
    public int mapIndex;
    public Sprite sprite;
}

//Used in the map selection screen, as well as loading a map scene
public class MapSelectionMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject mapsHolder, mapPreviewPrefab, selectedEffectObj;
    [SerializeField]
    private MapClass[] allMaps;
    private List<GameObject> allMapsList;
    public static Action<GameObject> setParent;

    void Start()
    {
        allMapsList = new List<GameObject>();
        //Creates a map object for each map in the list, then adds the objects in a list to reference later
        foreach (var map in allMaps)
        {
            GameObject obj = Instantiate(mapPreviewPrefab, mapsHolder.transform);
            obj.GetComponent<MapClassScript>().setMapDetails(map.sprite, map.MapName, map.mapScene.name, map.mapIndex);
            allMapsList.Add(obj);
        }
    }

    //Called when user clicks on a map preview, sets selected effect to the selected map
    public void selectMapEffect(GameObject obj)
    {
        selectedEffectObj.transform.SetParent(obj.transform, false);
        selectedEffectObj.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        selectedEffectObj.SetActive(true);
    }

    void OnEnable()
    {
        setParent += selectMapEffect;
    }

    void OnDisable()
    {
        setParent -= selectMapEffect;
    }
}