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

        private void Start() => CreateLayout(_Debug);

        private void CreateLayout(GameObject[] pCards)
        {
            int lLength = pCards.Length;
            for (int i = 0; i < lLength; i++)
            {
                Instantiate(_CardButtonPrefab, _Container).GetComponent<CustomCardButton>()
                                                          .Enable();
            }
        }

    }
}
