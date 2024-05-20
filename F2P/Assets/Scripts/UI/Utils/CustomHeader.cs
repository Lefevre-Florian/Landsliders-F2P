using TMPro;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI.Screens
{
    public class CustomHeader : MonoBehaviour
    {
        [Header("Monetization")]
        [SerializeField] private TextMeshProUGUI _UISoftCurrencyLabel = null;
        [SerializeField] private TextMeshProUGUI _UIHardCurrencyLabel = null;

        private void Start()
        {
            // Init the header with player save data
            UpdateHeader();

            Save.OnDataUpdated += UpdateHeader;
        }

        /// <summary>
        /// Update method for every label in the header
        /// </summary>
        private void UpdateHeader()
        {
            _UISoftCurrencyLabel.text = Save.data.softcurrency.ToString();
            _UIHardCurrencyLabel.text = Save.data.hardcurrency.ToString();
        }

        private void OnDestroy() => Save.OnDataUpdated -= UpdateHeader;
    }
}
