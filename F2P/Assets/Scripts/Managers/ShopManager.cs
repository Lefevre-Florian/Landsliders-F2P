using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing;

public class ShopManager : MonoBehaviour
{
    private int _HardCurrency = 0;

    public void BuyHardCurrency(UnityEngine.Purchasing.Product pProduct)
    {
        _HardCurrency += (int)pProduct.definition.payout.quantity;
        print(_HardCurrency);
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
