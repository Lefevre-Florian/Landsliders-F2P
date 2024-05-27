using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P.UI.Screens
{
    public class ShopConsentScreen : Screen
    {         
        public void OnConfirmPurchase()
        {
            if (ShopManager.GetInstance()._Softcurrecy)
            {
                Close();
                ShopManager.GetInstance()._Softcurrecy = false;
                ShopManager.GetInstance().BuySoftCurrency(ShopManager.GetInstance().pendingCurrency);
            }
            if (ShopManager.GetInstance()._Chest)
            {
                Close();
                ShopManager.GetInstance()._Chest = false;
                ShopManager.GetInstance().Chest(ShopManager.GetInstance().pendingFragments);
            }
            if (ShopManager.GetInstance()._RandomChest)
            {
                Close();
                ShopManager.GetInstance()._RandomChest = false;
                ShopManager.GetInstance().RandomChest();
            }
        }

        public void OnCancelPurchase()
        {
            Close();
            ShopManager.GetInstance().pendingFragments = 0;
            ShopManager.GetInstance().pendingCurrency = 0;
            ShopManager.GetInstance()._Softcurrecy = false;
            ShopManager.GetInstance()._Chest = false;
        }
    }
}
