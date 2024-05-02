using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI.Screens
{
    public class DeckScreen : Screen
    {
        [Header("Utils")]
        [SerializeField] private RectTransform _Container = null;
        [SerializeField] private GameObject _CardButtonPrefab = null;

        [SerializeField] private GameObject[] _Debug = new GameObject[0];

        // Variables
        private bool _Loaded = false;

        private void OnEnable()
        {
            if (!_Loaded)
            {
                _Loaded = true;
                CreateLayout(_Debug);
            }
        }

        private void CreateLayout(GameObject[] pCards)
        {
            CustomCardButton lCard;

            int lLength = Save.data.cards.Length;

            for (int i = 0; i < lLength; i++)
            {
                lCard = Instantiate(_CardButtonPrefab, _Container).GetComponent<CustomCardButton>();
                lCard.Enable(Save.data.cards[i], _Debug[i]);
            }
        }
    }
}
