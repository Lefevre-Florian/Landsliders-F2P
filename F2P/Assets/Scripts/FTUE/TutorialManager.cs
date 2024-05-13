using System;

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

        // Get & Set
        public FTUEPhaseSO CurrentPhase { get { return _Phase[_PhaseID - 1]; } }

        public int CurrentPhaseID { get { return _PhaseID; } }

        // Events
        public event Action OnPhaseUpdated;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(this);
                return;
            }
            _Instance = this;

            GameFlowManager.PlayerLoaded.AddListener(UpdatePlayer);
            GameFlowManager.HandLoaded.AddListener(UpdateHand);
        }

        private void UpdatePhase()
        {
            _PhaseID += 1;
            if (_PhaseID > _Phase.Length)
                _PhaseID = _Phase.Length;

            OnPhaseUpdated?.Invoke();
        }

        private void UpdatePlayer() => Player.GetInstance().SetPosition(CurrentPhase.StartPosition);

        private void UpdateHand()
        {
            print("ko");
            HandManager lHand = HandManager.GetInstance();
            lHand.CreateDeck(CurrentPhase.Deck);
            lHand.CreateHand(CurrentPhase.StartNBCards);
        }

        private void UpdateGrid()
        {

        }

        private void OnDestroy()
        {
            if (_Instance == this)
            {
                _Instance = null;

                GameFlowManager.PlayerLoaded.RemoveListener(UpdatePlayer);
                GameFlowManager.HandLoaded.RemoveListener(UpdateHand);
            }
        }
    }
}
