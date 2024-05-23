using UnityEngine;

namespace Com.IsartDigital.F2P.Biomes
{
    public class BiomeFeedback : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _Renderer = null;

        [Header("Feedbacks")]
        [SerializeField] private Material _CardLayer = null;
        [SerializeField] private Material _MoveLayer = null;

        // Variables
        private Material _BaseLayer = null;

        private Biome _Biome = null;

        private GameObject _AdditionalLayer = null;

        // Get & Set
        public Material CurrentLayer { get { return _Renderer.material; } }

        private void Start()
        {
            _BaseLayer = _Renderer.material;
            _Biome = GetComponent<Biome>();

            if (!_Biome.IsReady)
                _Biome.OnReady += Enable;
            else
                Enable();
        }

        public void DisplayAdditionalLayer(GameObject pAdditionalLayer)
        {
            if (_AdditionalLayer != null)
                Destroy(_AdditionalLayer);

            _AdditionalLayer = Instantiate(pAdditionalLayer, transform);
            _AdditionalLayer.transform.position = transform.position;
        }

        public void StopDisplayingAdditionalLayer()
        {
            if (_AdditionalLayer == null)
                return;

            Destroy(_AdditionalLayer);
            _AdditionalLayer = null;
        }

        private void Enable()
        {
            _Biome.OnReady -= Enable;

            _Biome.OnRemovableStateChanged += RemovableObserver;
            _Biome.OnWalkableStateChanged += WalkableObserver;

            WalkableObserver(_Biome.IsWalkable);
            RemovableObserver(_Biome.CanBeRemoved);

            if (!_Biome.IsWalkable 
                && GameManager.GetInstance().currentState == GameManager.State.MovingPlayer)
                EnableMoveLayer();
        }

        private void WalkableObserver(bool pState)
        {
            if (!pState)
            {
                GameManager.CardPlaced.AddListener(EnableMoveLayer);
                GameManager.PlayerMoved.AddListener(DisableMoveLayer);
            }
            else
            {
                GameManager.CardPlaced.RemoveListener(EnableMoveLayer);
                GameManager.PlayerMoved.RemoveListener(DisableMoveLayer);
            }
        }

        private void RemovableObserver(bool pState)
        {
            if (pState)
                TEMPCard.OnFocus -= CardFocus;
            else
                TEMPCard.OnFocus += CardFocus;

            CardFocus(!pState);
        }

        private void CardFocus(bool pFocusState)
        {
            if (pFocusState)
                _Renderer.material = _CardLayer;
            else
                _Renderer.material = _BaseLayer;
        }

        private void EnableMoveLayer() => _Renderer.material = _MoveLayer;

        private void DisableMoveLayer() => _Renderer.material = _BaseLayer;

        private void OnDestroy()
        {
            if(_Biome != null)
                _Biome.OnReady -= Enable;

            GameManager.CardPlaced.RemoveListener(EnableMoveLayer);
            GameManager.PlayerMoved.RemoveListener(DisableMoveLayer);

            TEMPCard.OnFocus -= CardFocus;
        }
    }
}
