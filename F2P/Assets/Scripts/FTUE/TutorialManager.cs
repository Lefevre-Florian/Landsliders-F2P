using com.isartdigital.f2p.gameplay.manager;
using Com.IsartDigital.F2P.Biomes;
using Com.IsartDigital.F2P.Biomes.Effects;

using System;
using System.Collections.Generic;
using System.Linq;

using Unity.VisualScripting;
using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FTUE
{
    public class TutorialManager : MonoBehaviour
    {
        #region Singleton
        private static TutorialManager _Instance = null;

        public static TutorialManager GetInstance()
        {
            if(_Instance == null) 
				_Instance = new TutorialManager();
            return _Instance;
        }

        private TutorialManager() : base() {}
        #endregion

        [Header("FTUE Flow")]
        [SerializeField][Range(1, 3)] private int _PhaseID = 1;
        [SerializeField] private FTUEPhaseSO[] _Phase = null;

        // Variables
        private GameManager _GameManager = null;
        private GridManager _GridManager = null;

        private int _TurnIdx = 0;

        private bool _Skip = false;

        // Get & Set
        public FTUEPhaseSO CurrentPhase { get { return _Phase[_PhaseID - 1]; } }

        public int CurrentPhaseID { get { return _Phase[_PhaseID - 1].FTUEPhase; } }

        public int Tick { get { return _TurnIdx; } }

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(this);
                return;
            }
            _Instance = this;

            GameFlowManager.PlayerLoaded.AddListener(UpdatePlayer);

            GameFlowManager.HandLoaded.AddListener(UpdateDeck);
            GameFlowManager.HandLoaded.AddListener(UpdateHand);
        }

        private void Start()
        {
            _GameManager = GameManager.GetInstance();
            _GameManager.OnAllEffectPlayed += PlayTurn;
            _GameManager.OnEffectPlayed += PlayEffect;

            _GridManager = GridManager.GetInstance();
        }

        public void UpdatePhase()
        {
            _PhaseID += 1;
            _TurnIdx = -1;

            if (_PhaseID > _Phase.Length)
                _PhaseID = _Phase.Length;
        }

        public void UpdatePlayer()
        {
            Player.GetInstance().SetPosition(CurrentPhase.StartPosition);
            if (CurrentPhaseID == 3)
                Player.GetInstance().AddComponent<DeckEffect>()
                                    .SetEffect(3, 1, DeckEffect.AlterationType.Negative);
        }

        private void UpdateHand() => HandManager.GetInstance().CreateHand(CurrentPhase.StartNBCards);

        private void UpdateDeck()
        {
            int lIdx = CurrentPhase.Decks.ToList().FindIndex(x => x.turn == _TurnIdx);
            if (lIdx == -1)
                return;

            int lLength = CurrentPhase.Decks[lIdx].deck.cards.Length;
            Tuple<BiomeType, int>[] lCards = new Tuple<BiomeType, int>[lLength];
            for (int i = 0; i < lLength; i++)
                lCards[i] = new Tuple<BiomeType, int>(CurrentPhase.Decks[lIdx].deck.cards[i].type,
                                                      CurrentPhase.Decks[lIdx].deck.cards[i].quantity);
            HandManager.GetInstance().CreateDeck(lCards);

            if (CurrentPhase.Decks[lIdx].updateHand)
                UpdateHand();
        }

        private void PlayTurn() 
        {
            _TurnIdx += 1;
            UpdateDeck();

            List<Phase> lPhases = CurrentPhase.Phases.ToList().FindAll(x => !x.isLinkedBiomeEffect && x.triggerTurn == _TurnIdx);
            if (lPhases != null && lPhases.Count > 0)
                UpdateGrid(lPhases);
        }

        private void PlayEffect(int pID)
        {
            List<Phase> lPhases = CurrentPhase.Phases.ToList().FindAll(x => x.isLinkedBiomeEffect && x.effectID == pID && x.triggerTurn == _TurnIdx);
            if (lPhases != null && lPhases.Count > 0)
                UpdateGrid(lPhases);
        }

        private void UpdateGrid(List<Phase> pPhases)
        {
            Biome lBiome;
            int lLength = pPhases.Count;
            for (int i = 0; i < lLength; i++)
            {
                _GridManager.ReplaceAtIndex(pPhases[i].position,
                                            CardPrefabDic.GetPrefab(pPhases[i].type).transform);
                lBiome = _GridManager.GetCardByGridCoordinate(pPhases[i].position);

                /// Allow us to fake every biome comportement
                if (lBiome.Type == BiomeType.volcan)
                {
                    lBiome.GetComponent<BiomeOracle>().Start();
                    lBiome.GetComponent<BiomeGridModifier>().Start();
                    lBiome.GetComponent<BiomeTimer>().SetCurrentTimer(1);
                    lBiome.GetComponent<BiomePlayerContact>().DisableCollision();
                }
                else if (lBiome.Type == BiomeType.desert || lBiome.Type == BiomeType.swamp)
                    lBiome.locked = true;
            }
        }

        private void OnDestroy()
        {
            if (_Instance == this)
            {
                _Instance = null;

                if (_GameManager != null)
                {
                    _GameManager.OnAllEffectPlayed -= PlayTurn;
                    _GameManager.OnEffectPlayed -= PlayEffect;
                }
                _GameManager = null;

                GameFlowManager.PlayerLoaded.RemoveListener(UpdatePlayer);

                GameFlowManager.HandLoaded.RemoveListener(UpdateDeck);
                GameFlowManager.HandLoaded.RemoveListener(UpdateHand);
            }
        }
    }
}
