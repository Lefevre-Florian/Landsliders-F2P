using UnityEngine;
using UnityEngine.UI;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI.Screens
{
    public class DeckScreen : Screen
    {
        private const float DISTANCE_TO_CAMERA = 6f;
        private const int TEXTURE_DEPTH_BUFFER = 16;

        [Header("Utils")]
        [SerializeField] private RectTransform _Container = null;
        [SerializeField] private GameObject _CardButtonPrefab = null;

        [Space(5)]
        [SerializeField] private Camera _RenderCamera = null;

        [SerializeField] private GameObject[] _Debug = new GameObject[0];

        private void Start() => CreateLayout(_Debug);

        private void CreateLayout(GameObject[] pCards)
        {
            _RenderCamera.gameObject.SetActive(true);
            CustomCardButton lCard;
            
            int lLength = pCards.Length;
            for (int i = 0; i < lLength; i++)
            {
                lCard = Instantiate(_CardButtonPrefab, _Container).GetComponent<CustomCardButton>();
                lCard.Enable();
                lCard.GetComponent<RawImage>().texture = CreateTextureBiome(310, 415, pCards[i]); ;
            }

            _RenderCamera.gameObject.SetActive(false);
        }

        /// <summary>
        /// Create a screenshot of the biomes to be displayed in UI
        /// </summary>
        /// <param name="pModel"></param>
        /// <param name="pVirtualTexture"></param>
        /// <returns></returns>
        private RenderTexture CreateTextureBiome(int pWidth, int pHeight, GameObject pModel)
        {
            RenderTexture lVirtualTexture = new RenderTexture(pWidth, pHeight, TEXTURE_DEPTH_BUFFER);
            lVirtualTexture.Create();

            GameObject lObj = Instantiate(pModel, _RenderCamera.transform);
            lObj.transform.localPosition = new Vector3(0f, 0f, DISTANCE_TO_CAMERA);

            _RenderCamera.targetTexture = lVirtualTexture;
            _RenderCamera.Render();

            lObj.SetActive(false);
            Destroy(lObj);

            _RenderCamera.targetTexture = null;            
            return lVirtualTexture;
        }
    }
}
