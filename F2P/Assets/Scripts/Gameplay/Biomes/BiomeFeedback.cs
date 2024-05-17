using UnityEngine;

namespace Com.IsartDigital.F2P.Biomes
{
    public class BiomeFeedback : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _Renderer = null;

        [Header("Feedbacks")]
        [SerializeField] private Sprite _CardLayer = null;
        [SerializeField] private Sprite _MoveLayer = null;

        // Variables
        private Sprite _BaseLayer = null;

        private Biome _Biome = null;

        // Get & Set
        public Sprite CurrentLayer { get { return _Renderer.sprite; } }

        private void Start()
        {
            _BaseLayer = _Renderer.sprite;
            _Biome = GetComponent<Biome>();

            if (!_Biome.IsReady)
                _Biome.OnReady += Enable;
            else
                Enable();
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
                _Renderer.sprite = _CardLayer;
            else
                _Renderer.sprite = _BaseLayer;
        }

        private void EnableMoveLayer() => _Renderer.sprite = _MoveLayer;

        private void DisableMoveLayer() => _Renderer.sprite = _BaseLayer;

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
