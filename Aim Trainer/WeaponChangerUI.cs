using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


[System.Serializable]
public class weaponPreview //A class to save different weapon preview states
{
    public GameObject gameObject;
    public string name;
}
[System.Serializable]
public class colorThemesPreview //A class to save different map themes
{
    public string name;
    public Color color;
    public Texture texture;
}
public class WeaponChangerUI : MonoBehaviour
{
    [SerializeField]
    private weaponPreview[] weapons;
    [SerializeField]
    private colorThemesPreview[] colorThemes;
    [SerializeField]
    Texture lightTex, darkTex;
    [SerializeField]
    private TextMeshProUGUI weaponName, equipBuyButton, equipBuyThemeButton;
    [SerializeField]
    private Image equipBuyButtonFrame, equipBuyButtonFrameThemes;
    [SerializeField]
    private GameObject[] skinsObjects, themesObjects;
    [SerializeField]
    private GameObject purchaseDetails;
    private int currentWep = 0, equippedWeapon, weaponSkinNum, previewThemeNum, equippedTheme;
    public string weaponPurchaseID, themePurchaseID, skinName = "Default", equippedWeaponID;
    public bool canEquip, canEquipTheme;
    public static Action<int, Color, Texture> changeTheme;
    public static Action<int, Color, Texture> saveTheme;
    public static Action<int> changeSkin, updateProductPrice;

    void Awake()
    {
        //Get saved player weapon, weapon skin, and map theme
        equippedWeaponID = PlayerPrefs.GetString("EquippedWeapon", "default");
        equippedWeapon = PlayerPrefs.GetInt("PlayerWeapon", 0);
        equippedTheme = PlayerPrefs.GetInt("Theme", 0);
        previewThemeNum = 0;
        skinName = "Default";
    }

    //Themes were added after implementation of weapon purchases, tried to merge into one but they had few differences and I was short on time.
    void UpdateProductIDWeapon() //Update product ID to match current viewed weapon in the shop, also checks for purchased status
    {
        //Custom string to pass as product ID for IAPManager which varies by item name
        weaponPurchaseID = "com.athaim.ath." + weaponName.text.Replace(" ", "").Replace("-", "").ToLower();
        if (!string.Equals(equippedWeaponID, weaponPurchaseID) || string.Equals(equippedWeaponID, "Default")) //Already equipped or starter weapon
        {
            if (CheckPurchase(weaponPurchaseID)) //Weapon already purchased, set to can equip it
            {
                SetButtonToEquip(equipBuyButton, equipBuyButtonFrame);
                canEquip = true;
                //Add equip mechanic
            } else { //Weapon not yet purchases, open buying window
                SetButtonToBuy(equipBuyButton, equipBuyButtonFrame);
                canEquip = false;
            }
        } else {
            SetButtonToEquipped(equipBuyButton, equipBuyButtonFrame);
        }
        if (updateProductPrice != null)
            updateProductPrice(savedPlayerData.instance.getProductPrice(weaponPurchaseID));
    }
    
    void UpdateProductIDTheme() //Update product ID to match current viewed map theme in the shop, also checks for purchased status
    { //Updates current productID to the selected theme
        //Custom string to pass as product ID for IAPManager which varies by item name
        themePurchaseID = "com.athaim.ath." + colorThemes[previewThemeNum].name.Replace(" ", "").ToLower() + "theme";
        if (previewThemeNum != equippedTheme) //Check if default theme is equipped
        {
            if (CheckPurchase(themePurchaseID)) //True if theme already purchased
            {
                SetButtonToEquip(equipBuyThemeButton, equipBuyButtonFrameThemes);
                canEquipTheme = true;
            } else { //Set to buy
                SetButtonToBuy(equipBuyThemeButton, equipBuyButtonFrameThemes);
                canEquipTheme = false;
            }
        } else {
            SetButtonToEquipped(equipBuyThemeButton, equipBuyButtonFrameThemes);
        }
        if (updateProductPrice != null)
            updateProductPrice(savedPlayerData.instance.getProductPrice(themePurchaseID));
    }

    public void EquipPurchaseProduct()
    {
        if (canEquip)
        {
            SetButtonToEquipped(equipBuyButton, equipBuyButtonFrame);
            equippedWeaponID = weaponPurchaseID;
            equippedWeapon = currentWep;
            PlayerPrefs.SetString("EquippedWeapon", weaponPurchaseID);
            PlayerPrefs.SetInt("WeaponSkin", weaponSkinNum); //Save equipped skin number
        } else {
            // IAPManager.instance.BuyProduct(weaponPurchaseID);
            savedPlayerData.instance.buyProduct(weaponPurchaseID, savedPlayerData.instance.getProductPrice(weaponPurchaseID)); //ID & Product Price
        }
    }
    
    public void BuyEquipTheme()
    {
        if (canEquipTheme)
        {
            PlayerPrefs.SetInt("Theme", previewThemeNum);
            equippedTheme = previewThemeNum;
            SetButtonToEquipped(equipBuyThemeButton, equipBuyButtonFrameThemes);
            if (previewThemeNum == 0)
            {
                saveTheme(previewThemeNum, colorThemes[previewThemeNum].color, lightTex); //Light mode
            } else {
                saveTheme(previewThemeNum, colorThemes[previewThemeNum].color, darkTex); //Dark mode
            }
        } else {
            // IAPManager.instance.BuyProduct(themePurchaseID);
            savedPlayerData.instance.buyProduct(themePurchaseID, savedPlayerData.instance.getProductPrice(weaponPurchaseID)); //ID & Product Price
        }
    }

    //Change button states and functionality, can be summed in one method name using Polymorphism + Enums for button state
    void SetButtonToEquip(TextMeshProUGUI button, Image buttonFrame)
    {
        button.text = "Equip";
        button.color = Color.white;
        buttonFrame.color = Color.white;
    }

    void SetButtonToEquipped(TextMeshProUGUI button, Image buttonFrame)
    {
        button.text = "Equipped!";
        button.color = Color.green;
        buttonFrame.color = Color.green;
    }

    void SetButtonToBuy(TextMeshProUGUI button, Image buttonFrame)
    {
        button.text = "Buy";
        button.color = Color.yellow;
        buttonFrame.color = Color.yellow;
    }

    void ChangeWeapon(int num) //Change previewed weapon
    {
        foreach (weaponPreview item in weapons)
        {
            item.gameObject.SetActive(false);
        }
        weapons[num].gameObject.SetActive(true);
        weaponName.text = weapons[num].name + " " + skinName;
        UpdateProductIDWeapon();
    }

    public void ChangeSkinName(string name) //Change previewed skin
    {
        skinName = name;
        weaponName.text = weapons[currentWep].name + " " + skinName;
        UpdateProductIDWeapon();
    }

    public void ChangeSkinNum(int num) //Called from buttons UI, each button has a unique number equivalent to skin order
    {
        weaponSkinNum = num;
    }

    public void ChangePreviewTheme(int themeNum)
    {
        weaponName.text = colorThemes[themeNum].name;
        previewThemeNum = themeNum;
        if (themeNum == 0)
        {
            changeTheme(themeNum, colorThemes[themeNum].color, lightTex); //Light mode
        } else {
            changeTheme(themeNum, colorThemes[themeNum].color, darkTex); //Dark mode
        }
        UpdateProductIDTheme();
    }

    public bool CheckPurchase(string productID) //Check if productID is already purchased
    {
        return savedPlayerData.instance.checkIfPurchased(productID);
    }

    //Cycle through all weapons/themes
    public void NextWeapon()
    {
        if (currentWep < weapons.Length - 1)
        {
            currentWep++;
        } else {
            currentWep = 0;
        }
        ChangeWeapon(currentWep);
    }

    public void PrevWeapon()
    {
        if (currentWep == 0)
        {
            currentWep = weapons.Length - 1;
        } else {
            currentWep--;
        }
        ChangeWeapon(currentWep);
    }

    public void ChangeToThemes() //Change preview mode from weapon skins to map themes (each one has a set of UI)
    {
        foreach (GameObject item in skinsObjects)
        {
            item.SetActive(false);
        }
        foreach (GameObject item in themesObjects)
        {
            item.SetActive(true);
        }
        updateProductPrice(0);
    }

    public void ChangeToSkins() //Change preview mode from map themes to weapon skins (each one has a set of UI)
    {
        foreach (GameObject item in skinsObjects)
        {
            item.SetActive(true);
        }
        foreach (GameObject item in themesObjects)
        {
            item.SetActive(false);
        }
        if (updateProductPrice != null)
                updateProductPrice(savedPlayerData.instance.getProductPrice(weaponPurchaseID));
    }

    public void EnablePurchaseDetails()
    {
        purchaseDetails.SetActive(true);
    }

    void OnEnable()
    {
        //Subscribe to saved data events
        savedPlayerData.UpdateProductStatus += UpdateProductIDWeapon;
        savedPlayerData.UpdateProductStatus += UpdateProductIDTheme;
        savedPlayerData.EnablePurchaseDetails += EnablePurchaseDetails;
        //Set defaults
        ChangeToSkins();
        ChangeSkinName("Default");
        ChangeWeapon(0);
        ChangePreviewTheme(0);
        changeSkin(0);
        purchaseDetails.SetActive(false);
    }

    void OnDisable()
    {
        //Subscribe to saved data events
        savedPlayerData.UpdateProductStatus -= UpdateProductIDWeapon;
        savedPlayerData.UpdateProductStatus -= UpdateProductIDTheme;
        savedPlayerData.EnablePurchaseDetails -= EnablePurchaseDetails;
        //Set defaults
        ChangeSkinName("Default");
        currentWep = 0;
        previewThemeNum = 0;
        ChangeWeapon(0);
        PlayerPrefs.SetInt("PlayerWeapon", equippedWeapon);
        ChangePreviewTheme(equippedTheme);
        //Save theme mode
        if (equippedTheme == 0)
        {
            saveTheme(equippedTheme, colorThemes[equippedTheme].color, lightTex); //Light mode
        } else {
            saveTheme(equippedTheme, colorThemes[equippedTheme].color, darkTex); //Dark mode
        }
    }
}
