using com.isartdigital.f2p.manager;
using Com.IsartDigital.F2P.Biomes;
using Com.IsartDigital.F2P.FTUE;

// Author (CR) : Lefevre Florian
namespace com.isartdigital.f2p.gameplay.quest
{
    public class QuestFTUE : Quests
    {
        private const int PHASE_ONE_MAX_TIMER = 2;

        private const int MAX_PHASE_TWO_CARD_FIELD = 3;
        private const int MAX_PHASE_THREE_SURVIVED = 3;

        // Variables
        private GameManager _GameManager = null;
        private TutorialManager _TutorialManager = null;
        
        // Variables (Phase - 1)
        private int _NbTurn = 0;

        // Variables (Phase - 2)
        private int _FieldHarvested = 0;
        private HandManager _HandManager = null;

        // Variables (Phase - 3)
        private bool _IsPoisoned = false;

        protected override void Start()
        {
            base.Start();

            _GameManager = GameManager.GetInstance();
            _TutorialManager = TutorialManager.GetInstance();

            _GameManager.OnTurnPassed += UnfoldPhase;
        }

        private void UnfoldPhase()
        {
            switch (_TutorialManager.CurrentPhaseID)
            {
                case 1:
                    _NbTurn += 1;
                    if(_NbTurn == PHASE_ONE_MAX_TIMER)
                    {
                        _NbTurn = 0;

                        _TutorialManager.UpdatePhase();
                        _HandManager = HandManager.GetInstance();
                    }
                    break;
                case 2:
                    if(_FieldHarvested == MAX_PHASE_TWO_CARD_FIELD)
                    {
                        _TutorialManager.UpdatePhase();

                        HandManager.OnDeckAltered.RemoveListener(ObserveFieldHarvesting);
                    }
                    break;
                case 3:

                    if (!_IsPoisoned)
                    {
                        _IsPoisoned = true;
                        _NbTurn = 0;
                        return;
                    }   

                    if (_IsPoisoned)
                    {
                        _NbTurn += 1;
                        print("Turn number : " + _NbTurn);

                        if(_NbTurn == MAX_PHASE_THREE_SURVIVED)
                        {
                            _TutorialManager.UpdatePhase();
                            QuestManager.ValidQuest.Invoke();
                        }
                    }

                    break;
            }
        }

        private void ObserveFieldHarvesting(int pNb, BiomeType pType)
        {
            if (pType == BiomeType.field)
                _FieldHarvested += 1;
        }

        private void OnDestroy()
        {
            if (_GameManager != null)
                _GameManager.OnTurnPassed -= UnfoldPhase;
            _GameManager = null;

            HandManager.OnDeckAltered.RemoveListener(ObserveFieldHarvesting);
        }
    }
}
