using Com.IsartDigital.F2P.FTUE.Dialogues;
using Com.IsartDigital.F2P.UI.UIHUD;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FTUE
{
    public class FTUEAdvice : MonoBehaviour
    {
        private const int ADVICE_ID_LAUNCH = 0;
        private const int ADVICE_ID_CARD_PLACED = 1;
        private const int ADVICE_ID_PLAYER_MOVED = 2;
        private const int ADVICE_ID_THIRD_PHASE_STARTED = 3;

        [SerializeField] private DialogueFlowSO _Advices = null;

        [Header("Hand Flow")]
        [SerializeField] private Animator _HandGameobject = null;

        [Space(5)]
        [SerializeField] private string _HandPointingAnimation = "";
        [SerializeField] private string _HandGrabbingAnimation = "";
        [SerializeField] private string _HandRequestMoveAnimation = "";

        // Variables
        private TutorialManager _TutorialManager = null;

        private bool _FirstMoved = false;
        private bool _FirstCardPlaced = false;

        private void Start()
        {
            _TutorialManager = TutorialManager.GetInstance();
            _TutorialManager.OnDialogueEnded += FirstAdvice;
            _TutorialManager.OnDialogueEnded += FinalAdvice;

            GameManager.CardPlaced.AddListener(SecondAdvice);
            GameManager.PlayerMoved.AddListener(ThirdAdvice);

            TEMPCard.OnFocus += VisualHandAdvice;
        }

        private void FirstAdvice()
        {
            if (_TutorialManager.CurrentPhaseID == 1 && _TutorialManager.Tick == 0)
            {
                _HandGameobject.SetTrigger(_HandPointingAnimation);
                CreateAdvice(ADVICE_ID_LAUNCH);
            }    
        }

        private void VisualHandAdvice(bool pState)
        {
            if(_TutorialManager.CurrentPhaseID == 1 && _TutorialManager.Tick == 0)
            {
                _HandGameobject.SetBool(_HandGrabbingAnimation, pState);
            }
        }

        private void SecondAdvice()
        {
            if (!_FirstCardPlaced)
            {
                _HandGameobject.SetBool(_HandRequestMoveAnimation, true);
                CreateAdvice(ADVICE_ID_CARD_PLACED);
                _FirstCardPlaced = true;
            }
        }

        private void ThirdAdvice()
        {
            if (!_FirstMoved)
            {
                Destroy(_HandGameobject.gameObject);

                CreateAdvice(ADVICE_ID_PLAYER_MOVED);
                _FirstMoved = true;
            }    
        }

        private void FinalAdvice()
        {
            if (_TutorialManager.CurrentPhaseID == 3 && _TutorialManager.Tick == 0)
                CreateAdvice(ADVICE_ID_THIRD_PHASE_STARTED);
        }

        private void CreateAdvice(int pID)
        {
            Instantiate(DialogueManager.GetInstance().GetDisplay(_Advices.Type), Hud.GetInstance().transform)
                                                     .GetComponent<DialogueWordPrinting>().SetDialogue(_Advices.Dialogues[pID]);
        }

        private void OnDestroy()
        {
            if(_TutorialManager != null)
            {
                _TutorialManager.OnDialogueEnded += FirstAdvice;
                _TutorialManager.OnDialogueEnded += FinalAdvice;
            }
            _TutorialManager = null;

            TEMPCard.OnFocus -= VisualHandAdvice;

            GameManager.CardPlaced.RemoveListener(SecondAdvice);
            GameManager.PlayerMoved.RemoveListener(ThirdAdvice);
        }
    }
}
