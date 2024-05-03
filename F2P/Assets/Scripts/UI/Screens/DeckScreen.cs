using Com.IsartDigital.F2P.FileSystem;
using System;
using UnityEngine;
using UnityEngine.UI;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI.Screens
{
    public class DeckScreen : Screen
    {
        private const string CMD_TOTAL_CARDS = "SELECT COUNT(id) FROM BIOME WHERE level = 1";

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

        private void Start()
        {
            GridLayoutGroup lLayout = _Container.GetComponent<GridLayoutGroup>();

            int lTotal = Convert.ToInt32(DatabaseManager.GetInstance().GetRow(CMD_TOTAL_CARDS)[0]);

            float lHeight = (lLayout.cellSize.y + lLayout.spacing.y) * (lTotal / lLayout.constraintCount);
            _Container.sizeDelta = new Vector2(_Container.sizeDelta.x, lHeight);
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
