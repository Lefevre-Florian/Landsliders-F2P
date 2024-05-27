using Com.IsartDigital.F2P.Biomes;
using Com.IsartDigital.F2P.FileSystem;

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI.Screens
{
    public class DeckScreen : Screen
    {
        private const string CMD_TOTAL_CARDS = "SELECT COUNT(id) FROM BIOME WHERE level = 1";

        [Serializable]
        public struct BiomeTexture
        {
            public BiomeType type;
            public Texture2D texture;
            public Sprite explication;
        }

        [Header("Utils")]
        [SerializeField] private RectTransform _Container = null;
        [SerializeField] private GameObject _CardButtonPrefab = null;

        [Space(5)]
        [SerializeField] private UpgradeScreen _UpgradeScreen = null;
        [SerializeField] private ConsentAskScreen _ConsentScreen = null;

        [Space(5)]
        [SerializeField] private BiomeTexture[] _CardTextures = new BiomeTexture[0];

        // Variables
        private bool _Loaded = false;

        private void OnEnable()
        {
            if (!_Loaded)
            {
                _Loaded = true;
                CreateLayout();
            }
        }

        private void Start()
        {
            GridLayoutGroup lLayout = _Container.GetComponent<GridLayoutGroup>();

            int lTotal = Convert.ToInt32(DatabaseManager.GetInstance().GetRow(CMD_TOTAL_CARDS)[0]);

            float lHeight = (lLayout.cellSize.y + lLayout.spacing.y) * (lTotal / lLayout.constraintCount);
            _Container.sizeDelta = new Vector2(_Container.sizeDelta.x, lHeight);
        }

        private void CreateLayout()
        {
            CustomCardButton lCard;

            List<BiomeTexture> lTextures = _CardTextures.ToList();
            int lLength = Save.data.cards.Length;

            BiomeType lCurrentType = default;
            for (int i = 0; i < lLength; i++)
            {
                lCurrentType = Save.data.cardPrefabs[i].GetComponent<Biome>().Type;

                lCard = Instantiate(_CardButtonPrefab, _Container).GetComponent<CustomCardButton>();
                lCard.Enable(Save.data.cards[i], 
                            lTextures.Find(x => x.type == lCurrentType).texture,
                            lTextures.Find(x => x.type == lCurrentType).explication,
                            _UpgradeScreen, 
                            _ConsentScreen);
            }
        }
    }
}
