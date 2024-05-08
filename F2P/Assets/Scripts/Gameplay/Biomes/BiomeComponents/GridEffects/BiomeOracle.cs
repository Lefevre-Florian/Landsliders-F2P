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

        // Variables
        private GameManager _GameManager = null;
        private GridManager _GridManager = null;

        private Biome _Biome = null;

        private Vector2[] _StackMemory = null;
        private Transform[] _Displays = null;

        private void Start()
        {
            if(_LayerContainer == null)
            {
                Debug.LogError(ERROR_LAYER);
                return;
            }

            _Biome = GetComponent<Biome>();
            if (_Biome.IsReady)
                Enable();
            else
                _Biome.OnReady += Enable;
        }

        private void Enable()
        {
            _Biome.OnReady -= Enable;

            _GameManager = GameManager.GetInstance();
            _GridManager = GridManager.GetInstance();

            _GameManager.OnTurnPassed += Predict;
        }

        private void Predict()
        {
            _StackMemory = GetComponent<IBiomeSupplier>().SupplyBiomes();

            if (_Displays != null)
                foreach (Transform lItem in _Displays)
                    Destroy(lItem);

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
