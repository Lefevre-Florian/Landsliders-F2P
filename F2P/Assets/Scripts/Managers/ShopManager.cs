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
using UnityEditor.Experimental.GraphView;
using System;

public class ShopManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _HardCurrencyLabel;
    [SerializeField]
    private GameObject _SoftCurrencyLabel;

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
            print("PurchaseComplet");
        }
        print("PurchaseFailed");
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
            DatabaseManager.GetInstance().WriteDataToSaveFile();
            print("PurchaseComplet");
        }
        else print("PurchasedFailed");    
    }

    public void DeckUpgrade()
    {
        if(Save.data.startingdecknb < 14)
        {
            Save.data.startingdecknb += 2;
            DatabaseManager.GetInstance().WriteDataToSaveFile();
        }
    }

    public void XPUpgrade()
    {
        if (!Save.data.xpdoubled)
        {
            Save.data.xpdoubled = true;
            DatabaseManager.GetInstance().WriteDataToSaveFile();
        }
    }

    public void Chest(int pFragments)
    {
        int pPrice;
        if (pFragments == _CommonShardNB) pPrice = _CommonChestPrice;
        else if (pFragments == _RareShardNB) pPrice = _RareChestPrice;
        else if (pFragments == _LegendaryShardNB) pPrice = _LegendaryChestPrice;
        else pPrice = 0;

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
            print("PurchaseComplet");
        }

        else print("PurchasedFailed");
        
    }

    public void Shard(int pFragments)
    {
        if (Save.data.softcurrency >= _DailyShardPrice)
        {
            int lCardLength = Save.data.cards.Length;
            int lChosenCard = UnityEngine.Random.Range(0, lCardLength);
            print(lChosenCard);
            Save.data.fragments[lChosenCard].fragment += pFragments;
            Save.data.softcurrency -= _DailyShardPrice;
            _SoftCurrencyLabel.GetComponentInChildren<TextMeshProUGUI>().text = Save.data.softcurrency.ToString();
            DatabaseManager.GetInstance().WriteDataToSaveFile();

            print("PurchaseComplet");
        }
        else print("PurchasedFailed");
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
            print("PurchaseComplet");
        }
        print("PurchaseFailed");
    }



}
