using com.isartdigital.f2p.gameplay.manager;

using System;

using UnityEngine;
using UnityEngine.Events;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    public class Biome : MonoBehaviour
    {
        [Header("Design")]
        [SerializeField][Min(0)] private int _Priority = 1;
        [SerializeField][Range(0f, 100f)] private float _EventProcChance = 50f;

        [SerializeField] public UnityEvent onTriggered = null;

        [Space(2)]
        [SerializeField] private BiomeType _Type = BiomeType.grassland;
        [SerializeField] private bool _CanBeReplaced = true;
        [SerializeField] private bool _IsWalkable = true;

        // Variables
        private GameManager _GameManager = null;

        private Vector2 _GridPosition = new Vector2();

        private bool _CanBeRemoved = true;

        // Get / Set
        public bool CanBeReplaced { get { return _CanBeReplaced; } }
        public bool IsWalkable { get { return _IsWalkable; } }
        public bool CanBeRemoved { get { return _CanBeRemoved; } }

        public Vector2 GridPosition { get { return _GridPosition; } }

        public BiomeType Type { get { return _Type; } }

        // Events
        public event Action OnReady;

        private void Start()
        {
            TEMPCard lCard = GetComponent<TEMPCard>();
            if (!lCard.isActiveAndEnabled)
                Enable();
            else
                lCard.OnPlaced += Enable;
        }

        public void PreciseSwitchWalkableState(bool pState) => _IsWalkable = pState;

        public void PreciseSwitchReplacabilityState(bool pState) => _CanBeReplaced = pState;

        public void PreciseSwitchRemoveByHandState(bool pState) => _CanBeRemoved = pState;

        public void Remove()
        {
            GridManager.GetInstance().RemoveAtIndex(_GridPosition);
            Destroy(gameObject);
        }

        public void Replace(Transform pSubsitutionBiome)
        {
            GridManager.GetInstance().ReplaceAtIndex(_GridPosition, pSubsitutionBiome);
            Destroy(gameObject);
        }

        private void Enable()
        {
            GetComponent<TEMPCard>().OnPlaced -= Enable;

            _GridPosition = GridManager.GetInstance()
                                       .GetGridCoordinate(transform.position);

            _GameManager = GameManager.GetInstance();
            if (_Priority != 0)
                _GameManager.OnEffectPlayed += TriggerPriority;

            OnReady?.Invoke();
        }

        private void TriggerPriority(int pGamePriority) 
        {
            if(pGamePriority == _Priority)
            {
                print("Play event : " + gameObject.name);
                onTriggered?.Invoke(); 
            }
        }

        private void OnDestroy()
        {
            GetComponent<TEMPCard>().OnPlaced -= Enable;

            if(_Priority != 0 && _GameManager != null)
                _GameManager.OnEffectPlayed -= TriggerPriority;

            onTriggered = null;
            _GameManager = null;
        }
    }
}
