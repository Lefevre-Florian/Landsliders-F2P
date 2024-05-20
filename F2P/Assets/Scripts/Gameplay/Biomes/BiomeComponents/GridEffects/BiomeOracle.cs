using com.isartdigital.f2p.gameplay.manager;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    [RequireComponent(typeof(Biome))]
    public class BiomeOracle : MonoBehaviour, IBiomeSupplier
    {
        private const string ERROR_LAYER = "Missing layer container";

        [Header("Feedbacks")]
        [SerializeField] private Transform _LayerContainer = null;
        [SerializeField] private GameObject _FeedbackLayer = null;

        [Header("Logic")]
        [SerializeField] private MonoBehaviour _SupplierComponent = null;

        [Space(5)]
        [SerializeField] private bool _StartAtReady = true;

        // Variables
        private GameManager _GameManager = null;
        private GridManager _GridManager = null;

        private Biome _Biome = null;

        private Vector2[] _StackMemory = null;
        private Transform[] _Displays = null;

        public void Start()
        {
            if(_LayerContainer == null)
            {
                Debug.LogError(ERROR_LAYER);
                return;
            }

            _Biome = GetComponent<Biome>();
            if (_StartAtReady)
            {
                if (_Biome.IsReady)
                    Enable();
                else
                    _Biome.OnReady += Enable;
            }
        }

        public void StartPrediction() => Enable();

        public void StopPrediction()
        {
            if (_Displays == null)
                return;

            int lLength = _Displays.Length;
            for (int i = 0; i < lLength; i++)
                Destroy(_Displays[i].gameObject);
            
            _GameManager.OnTurnPassed -= Predict;
        }

        private void Enable()
        {
            _Biome.OnReady -= Enable;

            _GameManager = GameManager.GetInstance();
            _GridManager = GridManager.GetInstance();

            _GameManager.OnTurnPassed += Predict;

            Predict();
        }

        private void Predict()
        {
            _StackMemory = (_SupplierComponent as IBiomeSupplier).SupplyBiomes();
            
            if (_Displays != null)
                foreach (Transform lItem in _Displays)
                    if(lItem != null)
                        Destroy(lItem.gameObject);

            if (_StackMemory == null || _StackMemory.Length == 0)
                return;

            int lLength = _StackMemory.Length;
            if (lLength == 0)
                return;

            _Displays = new Transform[lLength];

            for (int i = 0; i < lLength; i++)
            {
                _Displays[i] = Instantiate(_FeedbackLayer, _LayerContainer).transform;
                _Displays[i].position = _GridManager.GetWorldCoordinate(_StackMemory[i]);
            }
        }

        [HideInInspector]
        public Vector2[] SupplyBiomes() => _StackMemory;

        private void OnDestroy()
        {
            _GridManager = null;

            if(_GameManager != null)
            {
                _GameManager.OnTurnPassed -= Predict;
                _GameManager = null;
            }

            if (_Biome != null)
                _Biome.OnReady -= Enable;
        }
    }
}
