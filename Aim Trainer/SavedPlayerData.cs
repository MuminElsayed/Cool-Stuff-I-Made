using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//A class that saves/loads purchased data on game load, can be uploaded to the same database as the leaderboard or using Firebase, but I had to cut corners.
public class savedPlayerData : MonoBehaviour
{
    public static SavedPlayerData instance;

    public int currentCurrency = 0;
    [SerializeField]
    private List<string> playerPurchases = new List<string>{"com.athaim.ath.handgundefault", "com.athaim.ath.defaultthemetheme"}; //Default product IDs
    [SerializeField]
    private List<string> allPurchaseIDs; //List of all purchaseable IDs
    [SerializeField]
    private int[] allPurchasePrices; //Equal list of product prices, can be changed into a Dictionary, but values were automatically imported from excel (and I had to redo this whole system, thanks to IOS.)
    public static Action confirmRestoreAction, updateProductStatus, enablePurchaseDetails;
    public static Action<int> UpdateCurrencyAction;
    private string currency1000id = "com.ath.coins1000", currency2500id = "com.ath.coins2500", currency5000id = "com.ath.coins5000", currency10000id = "com.ath.coins10000"; //Currency product IDs

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public int GetProductPrice(string productID)
    {
        if (allPurchaseIDs.Contains(productID))
        {
            // print("price: " + productID + allPurchasePrices[allPurchaseIDs.IndexOf(productID)]);
            return allPurchasePrices[allPurchaseIDs.IndexOf(productID)];
        } else {
            print("product not found, id: " + productID);
            return 999999;
        }
    }

    public void AddToPurchases(string purchaseName)
    {
        playerPurchases.Add(purchaseName);
    }

    public bool CheckIfPurchased(string productID)
    {
        //Return purchase status of an item
        if (playerPurchases.Contains(productID))
        {
            return true;
        } else {
            return false;
        }
    }

    public void RestorePurchases()
    {
        playerPurchases.RemoveRange(2, playerPurchases.Count - 2);
        ResetCurrency();
        PlayerPrefs.SetInt("PlayerWeapon", 0);
        PlayerPrefs.SetInt("Theme", 0);
        PlayerPrefs.SetInt("WeaponSkin", 0);
        confirmRestoreAction();
    }

    //In game Currency
    public void AddCurrency(int number)
    {
        currentCurrency += number;
    }

    public void BuyProduct(string productID, int cost)
    {
        if (cost <= currentCurrency)
        {
            //Buy success
            savedPlayerData.instance.AddToPurchases(productID);
            currentCurrency -= cost;
            UpdateCurrency();
            enablePurchaseDetails();
            weaponPurchaseWindow.instance.purchaseSuccess();
            if (updateProductStatus != null)
                {
                    updateProductStatus();
                }
        } else {
            //Fail
            enablePurchaseDetails();
            weaponPurchaseWindow.instance.notEnoughCreds();
            print("not enough currency");
            // return "Buy failed: not enough credits.";
        }
    }

    public void Buy1000()
    {
        IAPManager.instance.BuyProduct(currency1000id);
    }
    public void Buy2500()
    {
        IAPManager.instance.BuyProduct(currency2500id);
    }
    public void Buy5000()
    {
        IAPManager.instance.BuyProduct(currency5000id);
    }
    public void Buy10000()
    {
        IAPManager.instance.BuyProduct(currency10000id);
    }

    public void ConfirmCurrencyPurchase(string currencyid)
    {
        if (string.Equals(currencyid, currency1000id))
        {
            AddCurrency(1000);
        } else if (string.Equals(currencyid, currency2500id))
        {
            AddCurrency(2500);
        } else if (string.Equals(currencyid, currency5000id))
        {
            AddCurrency(5000);
        } else if (string.Equals(currencyid, currency10000id))
        {
            AddCurrency(10000);
        }
    }

    private void ResetCurrency()
    {
        currentCurrency = 0;
    }

    public int GetCurrentCurrency()
    {
        return currentCurrency;
    }

    public void UpdateCurrency()
    {
        PlayerPrefs.SetInt("currency", currentCurrency);
        if (UpdateCurrencyAction != null)
            UpdateCurrencyAction(currentCurrency);
    }

    void OnEnable()
    {
        UpdateCurrency();
        savedPlayerData.updateProductStatus += UpdateCurrency;
        //Load player data
        JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString("savedPlayerData"), this);
    }

    void OnDisable()
    {
        savedPlayerData.updateProductStatus -= UpdateCurrency;
        //Save player data
        string jsonData = JsonUtility.ToJson(this, false);
        PlayerPrefs.SetString("savedPlayerData", jsonData);
        PlayerPrefs.Save();
    }
}
