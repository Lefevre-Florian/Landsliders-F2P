using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing;
using Com.IsartDigital.F2P;
using Com.IsartDigital.F2P.FileSystem;

public class ShopManager : MonoBehaviour
{
    public void BuyHardCurrency(UnityEngine.Purchasing.Product pProduct)
    {
        Save.data.hardcurrency += (int)pProduct.definition.payout.quantity;
        DatabaseManager.GetInstance().WriteDataToSaveFile();
        print(Save.data.hardcurrency);
    }

    public void OnPurchaseComplete(UnityEngine.Purchasing.Product pProduct)
    {
        if (pProduct.definition.type == ProductType.Consumable)
        {
            BuyHardCurrency(pProduct);
        }
    }

    public void OnPurchaseFailed(UnityEngine.Purchasing.Product pProduct, PurchaseFailureDescription pPurchaseFailureDescription)
    {
        print("The Purchase didn't complete");
    }
}
