using com.isartdigital.f2p.gameplay.manager;
using com.isartdigital.f2p.manager;

using Com.IsartDigital.F2P.Biomes;
using Com.IsartDigital.F2P.Biomes.Effects;
using Com.IsartDigital.F2P.FileSystem;
using Com.IsartDigital.F2P.FTUE.Dialogues;
using Com.IsartDigital.F2P.UI.UIHUD;

using System;
using System.Collections;
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

        #region Tracking
        private const string TRACKER_NAME = "ftueComplete";
        private const string TRACKER_DURATION_PARAMETER_NAME = "timeInSecondMinute";
        #endregion

        [Header("FTUE Flow")]
        [SerializeField][Range(1, 3)] private int _PhaseID = 1;
        [SerializeField] private FTUEPhaseSO[] _Phase = null;

        [Space(5)]
        [SerializeField] private DialogueLinePrinting _StoryNarrator = null;
        [SerializeField] private DialogueFlowSO _EndDialogue = null;

        // Variables
        private GameManager _GameManager = null;
        private GridManager _GridManager = null;

        private int _TurnIdx = 0;

        private DialogueWordPrinting _CurrentTextBox = null;
        private int _DialoguePhaseIdx = 0;

        private DateTime _FTUEStartTime = default;

        private bool _NarrationSkipped = true;

        // Get & Set
        public FTUEPhaseSO CurrentPhase { get { return _Phase[_PhaseID - 1]; } }

        public int CurrentPhaseID { get { return _Phase[_PhaseID - 1].FTUEPhase; } }

        public int Tick { get { return _TurnIdx; } }

        public DateTime StartTime { get { return _FTUEStartTime; } }

        // Events
        public event Action OnDialogueEnded;

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

            QuestManager.ValidQuest.AddListener(EndFTUE);

            _FTUEStartTime = DateTime.UtcNow;

            if (_StoryNarrator.isActiveAndEnabled)
            {
                _NarrationSkipped = false;
                _StoryNarrator.OnDialogueEnded.AddListener(StartFTUE);
            }
            else
            {
                StartFTUE();
            }

            StartCoroutine(SetPlayerFtue());
        }

        IEnumerator SetPlayerFtue()
        {
            yield return new WaitForEndOfFrame();
            Player.GetInstance()._InFTUE = true;
        }

        public void UpdatePhase()
        {
            _PhaseID += 1;
            _TurnIdx = -1;
            _DialoguePhaseIdx = 0;

            if (_PhaseID > _Phase.Length)
                _PhaseID = _Phase.Length;

            UpdateDialogue();
        }

        #region FTUE Gampelay
        public void UpdatePlayer()
        {
            if (CurrentPhaseID == 1 && _TurnIdx == 0 && !_NarrationSkipped)
                Player.GetInstance().gameObject.SetActive(false); 

            Player.GetInstance().SetPosition(CurrentPhase.StartPosition);
            if (CurrentPhaseID == 3)
                Player.GetInstance().AddComponent<DeckEffect>()
                                    .SetEffect(3, 1, DeckEffect.AlterationType.Negative);
        }

        private void UpdateHand()
        {
            HandManager.GetInstance().CreateHand(CurrentPhase.StartNBCards);
            if (!(CurrentPhaseID == 1 && _TurnIdx == 0))
                Hud.GetInstance().Lifebar.UpdateHealth();
        }

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

            if (!(CurrentPhaseID == 1 && _TurnIdx == 0))
                Hud.GetInstance().Lifebar.UpdateHealth();

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
                    lBiome.Lock();
            }
        }
        #endregion

        #region FTUE Dialogue and Juiciness
        /// <summary>
        /// Played after the narration panel (if active)
        /// </summary>
        private void StartFTUE()
        {
            Player.GetInstance().gameObject.SetActive(true);
            UpdateDialogue();
        }

        private void UpdateDialogue() 
        {
            // Dialogues
            GameObject lTextBox = Instantiate(DialogueManager.GetInstance().GetDisplay(CurrentPhase.DialogueFlow[_DialoguePhaseIdx].Type), Hud.GetInstance().transform);
            _CurrentTextBox = lTextBox.GetComponent<DialogueWordPrinting>();
            _CurrentTextBox.SetDialogues(CurrentPhase.DialogueFlow[_DialoguePhaseIdx].Dialogues,
                                         CurrentPhase.DialogueFlow[_DialoguePhaseIdx].Tween);

            if (CurrentPhase.DialogueFlow.Length > 1)
                _CurrentTextBox.OnDialogueEnded.AddListener(ManageDialoguePhaseFlow);
            else
                _CurrentTextBox.OnDialogueEnded.AddListener(ConversationEnd);
        }

        private void ManageDialoguePhaseFlow()
        {
            _CurrentTextBox.OnDialogueEnded.RemoveListener(ManageDialoguePhaseFlow);
            _CurrentTextBox = null;

            _DialoguePhaseIdx += 1;
            if (_DialoguePhaseIdx >= CurrentPhase.DialogueFlow.Length)
            {
                OnDialogueEnded?.Invoke();
                return;
            }
               
            UpdateDialogue();
        }

        private void ConversationEnd()
        {
            if (_CurrentTextBox != null)
                _CurrentTextBox.OnDialogueEnded.RemoveListener(ConversationEnd);
            _CurrentTextBox = null;

            OnDialogueEnded?.Invoke();
        }
        #endregion

        private void EndFTUE()
        {
            // Last dialogue
            GameObject lTextBox = Instantiate(DialogueManager.GetInstance().GetDisplay(_EndDialogue.Type), Hud.GetInstance().transform);
            lTextBox.GetComponent<DialogueWordPrinting>().SetDialogues(_EndDialogue.Dialogues,
                                                                       _EndDialogue.Tween);

            // Save
            Save.data.ftuecomplete = true;
            DatabaseManager.GetInstance().WriteDataToSaveFile();

            // Tracking
            TimeSpan lDuration = (DateTime.UtcNow - _FTUEStartTime).Duration();
            DataTracker.GetInstance().SendAnalytics(TRACKER_NAME, new Dictionary<string, object>() { { TRACKER_DURATION_PARAMETER_NAME, lDuration.Minutes + ":" + lDuration.Seconds } });
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
                _GridManager = null;

                GameFlowManager.PlayerLoaded.RemoveListener(UpdatePlayer);

                GameFlowManager.HandLoaded.RemoveListener(UpdateDeck);
                GameFlowManager.HandLoaded.RemoveListener(UpdateHand);

                QuestManager.ValidQuest.RemoveListener(EndFTUE);

                if (_StoryNarrator != null)
                    _StoryNarrator.OnDialogueEnded.RemoveListener(StartFTUE);
            }
        }
    }
}
