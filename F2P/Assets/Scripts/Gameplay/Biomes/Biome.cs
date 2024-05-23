using com.isartdigital.f2p.gameplay.manager;
using Com.IsartDigital.F2P.Cards;
using Com.IsartDigital.F2P.Sound;
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
        [SerializeField] private bool _CanBeRemoved = true;

        [Header("Sound")]
        [SerializeField] private SoundEmitter _AwakeSoundEmitter = null;

        // Variables
        private GameManager _GameManager = null;

        private Vector2 _GridPosition = new Vector2();

        private CardRenderer _Renderer = null;

        [HideInInspector] public bool locked = false;

        // Get / Set
        public bool CanBeReplaced { get { return _CanBeReplaced; } }                        // Dfine if a card can be change by another biome or event
        public bool IsWalkable { get { return _IsWalkable; } }                              // Define if the player can move on it
        public bool CanBeRemoved { get { return _CanBeRemoved; } }                          // Define if the biome can be remove by the a card in the hand

        public Vector2 GridPosition { get { return _GridPosition; } }

        public BiomeType Type { get { return _Type; } }

        public bool IsReady { get { return !GetComponent<TEMPCard>().isActiveAndEnabled; } }

        // Events
        public event Action OnReady;

        public event Action<bool> OnWalkableStateChanged;
        public event Action<bool> OnRemovableStateChanged;

        private void Start()
        {
            TEMPCard lCard = GetComponent<TEMPCard>();

            if (!lCard.isActiveAndEnabled)
                Enable();
            else
            {
                lCard.OnPlaced += Enable;
                if (_AwakeSoundEmitter != null)
                    lCard.OnPlaced += PlaySoundAwake;
            }
                
        }

        public void PreciseSwitchWalkableState(bool pState)
        {
            OnWalkableStateChanged?.Invoke(pState);
            _IsWalkable = pState;
        }

        public void PreciseSwitchReplacabilityState(bool pState) => _CanBeReplaced = pState;

        public void PreciseSwitchRemoveByHandState(bool pState)
        {
            OnRemovableStateChanged?.Invoke(pState);
            _CanBeRemoved = pState;
        }

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

            _Renderer = transform.GetChild(0)
                                 .GetComponent<CardRenderer>();

            print(gameObject.name + _Renderer);
            if(_Renderer != null)
            {
                _Renderer.EnableAnimation();
                _Renderer.SetSortingLayer(-(int)(_GridPosition.x + _GridPosition.y));
            }
            OnReady?.Invoke();
        }

        private void PlaySoundAwake() => _AwakeSoundEmitter.PlaySFXOnShot();

        private void TriggerPriority(int pGamePriority) 
        {
            if(pGamePriority == _Priority && !locked)
                onTriggered?.Invoke(); 
        }

        private void OnDestroy()
        {
            GetComponent<TEMPCard>().OnPlaced -= Enable;

            if (_AwakeSoundEmitter != null)
                GetComponent<TEMPCard>().OnPlaced -= PlaySoundAwake;

            if(_Priority != 0 && _GameManager != null)
                _GameManager.OnEffectPlayed -= TriggerPriority;

            onTriggered = null;
            _GameManager = null;
        }
    }
}
