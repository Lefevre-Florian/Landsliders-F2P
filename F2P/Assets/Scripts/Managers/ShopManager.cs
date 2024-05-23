using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing;
using UnityEditor;
using Com.IsartDigital.F2P;
using Com.IsartDigital.F2P.FileSystem;
using System.Reflection.Emit;
using TMPro;
using System;
using UnityEngine.UI;
using Com.IsartDigital.F2P.UI.Screens;

public class ShopManager : MonoBehaviour
{

    #region Singleton
    private static ShopManager _Instance = null;

    public static ShopManager GetInstance()
    {
        if (_Instance == null)
            _Instance = new ShopManager();
        return _Instance;
    }

    private ShopManager() : base() { }
    #endregion

    [SerializeField]
    private GameObject _HardCurrencyLabel;
    [SerializeField]
    private GameObject _SoftCurrencyLabel;

    private GameObject _ButtonObject;

    [HideInInspector] public int pendingFragments;
    [HideInInspector] public int pendingCurrency;
    [HideInInspector] public bool _Softcurrecy;
    [HideInInspector] public bool _Chest;
    [HideInInspector] public bool _RandomChest;

    [SerializeField]private ShopConsentScreen _ConsentScreen;

    [SerializeField] private GameObject _XPDoubledObject;
    [SerializeField] private GameObject _DeckUpgradeObject;
    [SerializeField] private GameObject _FreeClaimObject;
    [SerializeField] private GameObject _XPMoneyIcon;
    [SerializeField] private GameObject _DeckMoneyIcon;


    private const string BOUGHTSTRING = "Bought";

    [SerializeField] private int _CommonChestPrice;
    [SerializeField] private int _RareChestPrice;
    [SerializeField] private int _LegendaryChestPrice;
    [SerializeField] private int _LuckyChestPrice;
    [SerializeField] private int _DailyShardPrice;
    [SerializeField] private int _DailyChestShardPrice;

    [SerializeField] private int _FirstSCPrice;
    [SerializeField] private int _SecondSCPrice;
    [SerializeField] private int _ThirdSCPrice;
    [SerializeField] private int _FourthSCPrice;
    [SerializeField] private int _FifthSCPrice;
    [SerializeField] private int _SixthSCPrice;

    
    [SerializeField] private int _CommonShardNB;
    [SerializeField] private int _RareShardNB;
    [SerializeField] private int _LegendaryShardNB;
    [SerializeField] private int _DailyChestShardNB;


    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(this);
            return;
        }
        _Instance = this;
    }
    private void Start()
    {
        if (Save.data.xpdoubled)
        {
            _XPDoubledObject.transform.GetChild(0).GetComponent<Button>().enabled = false;
            _XPDoubledObject.GetComponentInChildren<TextMeshProUGUI>().text = BOUGHTSTRING;
            _XPMoneyIcon.gameObject.SetActive(false);
        }
        if (Save.data.startingdecknb == 14)
        {
            _DeckUpgradeObject.transform.GetChild(0).GetComponent<Button>().enabled = false;
            _DeckUpgradeObject.GetComponentInChildren<TextMeshProUGUI>().text = BOUGHTSTRING;
            _DeckMoneyIcon.gameObject.SetActive(false);
        }
        if (Save.data.freegiftclaim)
        {
            _FreeClaimObject.GetComponent<CountDown>().enabled = true;
            _FreeClaimObject.GetComponentInChildren<TextMeshProUGUI>().text = _FreeClaimObject.GetComponent<CountDown>().countdownText.ToString();
            _FreeClaimObject.GetComponent<Button>().enabled = false;
        }
    }

    public void ShowConfirmationChest(int pFragments)
    {
        pendingFragments = pFragments;
        _Chest = true;
        _ConsentScreen.Open();
    }

    public void ShowConfirmationRandomChest()
    {
        _RandomChest = true;
        _ConsentScreen.Open();
    }

    public void ShowConfirmationSoftCurrency(int pSoftCurrency)
    {
        pendingCurrency = pSoftCurrency;
        _Softcurrecy = true;
        _ConsentScreen.Open();
    }

    public void BuyHardCurrency(int pHardCurrency)
    {
        Save.data.hardcurrency += pHardCurrency;
        _HardCurrencyLabel.GetComponentInChildren<TextMeshProUGUI>().text = Save.data.hardcurrency.ToString();
        DatabaseManager.GetInstance().WriteDataToSaveFile();
    }

    public void BuySoftCurrency(int pSoftCurrency)
    {
        int pPrice = 0;
        if (pSoftCurrency == _FirstSCPrice ) pPrice = _FirstSCPrice;
        else if (pSoftCurrency == _SecondSCPrice ) pPrice = _SecondSCPrice;
        else if (pSoftCurrency == _ThirdSCPrice) pPrice = _ThirdSCPrice;
        else if (pSoftCurrency == _FourthSCPrice) pPrice = _FourthSCPrice;
        else if (pSoftCurrency == _FifthSCPrice) pPrice = _FifthSCPrice;
        else if (pSoftCurrency == _SixthSCPrice) pPrice = _SixthSCPrice;
        if (Save.data.hardcurrency >= pPrice)
        {
            Save.data.softcurrency += pSoftCurrency * 3;
            Save.data.hardcurrency -= pPrice;
            _SoftCurrencyLabel.GetComponentInChildren<TextMeshProUGUI>().text = Save.data.softcurrency.ToString();
            _HardCurrencyLabel.GetComponentInChildren<TextMeshProUGUI>().text = Save.data.hardcurrency.ToString();
            DatabaseManager.GetInstance().WriteDataToSaveFile();
            
        }
        
    }

    public void DailyChest(int pFragments)
    {
        if(Save.data.softcurrency >= _DailyChestShardPrice)
        {
            int lCardLength = Save.data.cards.Length;
            while (pFragments > 0)
            {
                int lChosenCard = UnityEngine.Random.Range(0, lCardLength);
                int lRandomFragments = UnityEngine.Random.Range(1, pFragments);
                Save.data.fragments[lChosenCard].fragment += lRandomFragments;
                pFragments -= lRandomFragments;
            }
            Save.data.softcurrency -= _DailyChestShardPrice;
            _SoftCurrencyLabel.GetComponentInChildren<TextMeshProUGUI>().text = Save.data.softcurrency.ToString();
            _ButtonObject.GetComponent<CountDown>().enabled = true;
            _ButtonObject.transform.parent.GetChild(1).gameObject.SetActive(false);
            _ButtonObject.GetComponentInChildren<TextMeshProUGUI>().text = _FreeClaimObject.GetComponent<CountDown>().countdownText.ToString();
            _ButtonObject.GetComponent<Button>().enabled = false;

            DatabaseManager.GetInstance().WriteDataToSaveFile();
            
        }
    }

    public void GetButtonObject(GameObject pGameObject)
    {
        _ButtonObject = pGameObject;
    }

    public void DeckUpgrade()
    {
        if(Save.data.startingdecknb < 14)
        {
            Save.data.startingdecknb += 2;
            _DeckUpgradeObject.transform.GetChild(0).GetComponent<Button>().enabled = false;
            _DeckUpgradeObject.GetComponentInChildren<TextMeshProUGUI>().text = BOUGHTSTRING;
            _DeckMoneyIcon.gameObject.SetActive(false);
            DatabaseManager.GetInstance().WriteDataToSaveFile();
        }
    }

    public void XPUpgrade()
    {
        if (!Save.data.xpdoubled)
        {
            Save.data.xpdoubled = true;
            _XPDoubledObject.transform.GetChild(0).GetComponent<Button>().enabled = false;
            _XPDoubledObject.GetComponentInChildren<TextMeshProUGUI>().text = BOUGHTSTRING;
            _XPMoneyIcon.gameObject.SetActive(false);
            DatabaseManager.GetInstance().WriteDataToSaveFile();
        }
    }

    

    public void Chest(int pFragments)
    {
        int pPrice = 0;
        if (pFragments == _CommonShardNB) pPrice = _CommonChestPrice;
        else if (pFragments == _RareShardNB) pPrice = _RareChestPrice;
        else if (pFragments == _LegendaryShardNB) pPrice = _LegendaryChestPrice;
        if (Save.data.hardcurrency >= pPrice)
        {
            int lCardLength = Save.data.cards.Length;
            while (pFragments > 0)
            {
                int lChosenCard = UnityEngine.Random.Range(0, lCardLength);
                int lRandomFragments = UnityEngine.Random.Range(1, pFragments);
                Save.data.fragments[lChosenCard].fragment += lRandomFragments;
                pFragments -= lRandomFragments;
            }
            Save.data.hardcurrency -= pPrice;
            _HardCurrencyLabel.GetComponentInChildren<TextMeshProUGUI>().text = Save.data.hardcurrency.ToString();
            DatabaseManager.GetInstance().WriteDataToSaveFile();
   
        }
    }

    public void Shard(int pFragments)
    {
        if (Save.data.softcurrency >= _DailyShardPrice)
        {
            int lCardLength = Save.data.cards.Length;
            int lChosenCard = UnityEngine.Random.Range(0, lCardLength);
            
            Save.data.fragments[lChosenCard].fragment += pFragments;
            Save.data.softcurrency -= _DailyShardPrice;
            _SoftCurrencyLabel.GetComponentInChildren<TextMeshProUGUI>().text = Save.data.softcurrency.ToString();
            _ButtonObject.GetComponent<CountDown>().enabled = true;
            _ButtonObject.transform.GetChild(0).gameObject.SetActive(false);
            _ButtonObject.GetComponentInChildren<TextMeshProUGUI>().text = _FreeClaimObject.GetComponent<CountDown>().countdownText.ToString();
            _ButtonObject.GetComponent<Button>().enabled = false;
            DatabaseManager.GetInstance().WriteDataToSaveFile();
        }
        
    }

    public void FreeClaim(int pFragments)
    {
        int lCardLength = Save.data.cards.Length;
        while (pFragments > 0)
        {
            int lChosenCard = UnityEngine.Random.Range(0, lCardLength);
            int lRandomFragments = UnityEngine.Random.Range(1, pFragments);
            Save.data.fragments[lChosenCard].fragment += lRandomFragments;
            pFragments -= lRandomFragments;
        }
        Save.data.freegiftclaim = true;
        _FreeClaimObject.GetComponent<CountDown>().enabled = true;
        _FreeClaimObject.GetComponentInChildren<TextMeshProUGUI>().text = _FreeClaimObject.GetComponent<CountDown>().countdownText.ToString();
        _FreeClaimObject.GetComponent<Button>().enabled = false;
        DatabaseManager.GetInstance().WriteDataToSaveFile();
    }

    public void RandomChest()
    {
        if (Save.data.hardcurrency >= _LuckyChestPrice)
        {
            int lCardLength = Save.data.cards.Length;
            int pFragment;
            for (int i = 0; i < 2; i++)
            {
                int lRandomChest = UnityEngine.Random.Range(0, 100);
                if (lRandomChest < 60)
                {
                    pFragment = 25;
                    while (pFragment > 0)
                    {
                        int lChosenCard = UnityEngine.Random.Range(0, lCardLength);
                        int lRandomFragments = UnityEngine.Random.Range(1, pFragment);
                        Save.data.fragments[lChosenCard].fragment += lRandomFragments;
                        pFragment -= lRandomFragments;
                    }
                }
                else if (lRandomChest >= 60 && lRandomChest < 90) 
                {
                    pFragment = 50;
                    while (pFragment > 0)
                    {
                        int lChosenCard = UnityEngine.Random.Range(0, lCardLength);
                        int lRandomFragments = UnityEngine.Random.Range(1, pFragment);
                        Save.data.fragments[lChosenCard].fragment += lRandomFragments;
                        pFragment -= lRandomFragments;
                    }
                }
                else if (lRandomChest >= 90)
                {
                    pFragment = 100;
                    while (pFragment > 0)
                    {
                        int lChosenCard = UnityEngine.Random.Range(0, lCardLength);
                        int lRandomFragments = UnityEngine.Random.Range(1, pFragment);
                        Save.data.fragments[lChosenCard].fragment += lRandomFragments;
                        pFragment -= lRandomFragments;
                    }
                }
                
            }
            Save.data.hardcurrency -= _LuckyChestPrice;
            _HardCurrencyLabel.GetComponentInChildren<TextMeshProUGUI>().text = Save.data.hardcurrency.ToString();
            DatabaseManager.GetInstance().WriteDataToSaveFile();    
            
        }
        
    }

  

}
