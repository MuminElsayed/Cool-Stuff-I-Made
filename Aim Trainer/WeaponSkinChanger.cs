using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that saves skin textures and changes current skin during runtime
[System.Serializable]
public class Skin //Skin properties
{
    public Texture texture;
    public Color color {get; set;} = Color.white;
    public float emissionIntensity {get; set;} = 1f;
}

[System.Serializable]
public class weaponParts //Each weapon part had a different texture
{
    public Skin[] skins;
    public Renderer renderer {get; set;}
}
public class WeaponSkinChanger : MonoBehaviour
{
    public int currentSkin;
    [SerializeField]
    private weaponParts[] weaponParts;

    void OnEnable()
    {
        currentSkin = PlayerPrefs.GetInt("WeaponSkin", 0);

        ChangeSkin(currentSkin);
        
        WeaponChangerUI.ChangeSkin += ChangeSkin;
    }

    public void ChangeSkin(int skinNum)
    {
        foreach (weaponParts part in weaponParts)
        {
            //Create temp material and set changes, changing material properties during runtime was not working.
            Material tempRend = part.renderer.material;
            //Set colors
            tempRend.SetColor("_BaseColor", part.skins[skinNum].color);
            tempRend.SetColor("_Color", part.skins[skinNum].color);

            //Set textures
            tempRend.SetTexture("_BaseMap", part.skins[skinNum].texture);
            tempRend.SetTexture("_EmissionMap", part.skins[skinNum].texture);

            //Set emission
            tempRend.SetColor("_EmissionColor", part.skins[skinNum].color * part.skins[skinNum].emissionIntensity);

            part.renderer.material = tempRend; //Set changes to material
        }
    }

    void OnDisable()
    {
        WeaponChangerUI.ChangeSkin -= ChangeSkin;
    }
}
