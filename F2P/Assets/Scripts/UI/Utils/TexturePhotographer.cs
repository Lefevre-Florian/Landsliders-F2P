using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI
{
    public class TexturePhotographer : MonoBehaviour
    {
        #region Singleton
        private static TexturePhotographer _Instance = null;

        public static TexturePhotographer GetInstance()
        {
            if(_Instance == null) 
				_Instance = new TexturePhotographer();
            return _Instance;
        }

        private TexturePhotographer() : base() {}
        #endregion

        #if UNITY_EDITOR
        private const string STREAMING_ERROR_MSG = "An object is already being rendered, only one object can be rendered at a time.";
        #endif

        private const float DISTANCE_TO_LENS = 6f;

        private const int TEXTURE_DEPTH_BUFFER = 16;

        // Variables
        private Camera _Camera = null;

        private Transform _StreamedObject = null;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(this);
                return;
            }
            _Instance = this;

            _Camera = GetComponent<Camera>();
            _Camera.enabled = false;
        }

        /// <summary>
        /// Create a screenshot of the biomes to be displayed in UI
        /// </summary>
        /// <param name="pModel"></param>
        /// <param name="pVirtualTexture"></param>
        /// <returns></returns>
        public RenderTexture CreateTextureBiome(int pWidth, int pHeight, GameObject pModel, Vector2 pScale = default)
        {
            _Camera.enabled = true;
            RenderTexture lVirtualTexture = new RenderTexture(pWidth, pHeight, TEXTURE_DEPTH_BUFFER);
            lVirtualTexture.Create();

            GameObject lObj = Instantiate(pModel, transform);
            lObj.transform.localPosition = new Vector3(0f, 0f, DISTANCE_TO_LENS);
            lObj.transform.localScale = pScale;

            _Camera.targetTexture = lVirtualTexture;
            _Camera.Render();

            lObj.SetActive(false);
            Destroy(lObj);

            _Camera.targetTexture = null;
            _Camera.enabled = false;

            return lVirtualTexture;
        }


        public RenderTexture CreateTextureBiome(Vector2 pSize, GameObject pModel, Vector2 pScale = default)
        {
            return CreateTextureBiome((int)pSize.x, (int)pSize.y, pModel, pScale);
        }

        public RenderTexture CreateEmptyTexture(Vector2 pSize) => new RenderTexture((int) pSize.x, (int) pSize.y, TEXTURE_DEPTH_BUFFER);

        public GameObject StartRecording(RenderTexture pTexture, GameObject pModel, Vector2 pScale = default)
        {
            if (_StreamedObject != null)
            {
                #if UNITY_EDITOR
                Debug.LogError(STREAMING_ERROR_MSG);
                #endif
                return null;
            }

            _Camera.enabled = true;
            _Camera.targetTexture = pTexture;

            _StreamedObject = Instantiate(pModel, transform).transform;
            _StreamedObject.position = new Vector3(0f, 0f, DISTANCE_TO_LENS);
            _StreamedObject.localScale = (pScale == default) ? new Vector2(1f, 1f) : pScale;

            return _StreamedObject.gameObject;
        }

        public void StopRecording()
        {
            if (_StreamedObject == null)
                return;

            Destroy(_StreamedObject.gameObject);
            _StreamedObject = null;

            _Camera.enabled = false;
            _Camera.targetTexture = null;
        }

        private void OnDestroy()
        {
            if (_Instance == this)
                _Instance = null;
        }
    }
}
