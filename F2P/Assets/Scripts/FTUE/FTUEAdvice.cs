using Com.IsartDigital.F2P.FTUE.Dialogues;

using TMPro;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FTUE
{
    public class FTUEAdvice : MonoBehaviour
    {
        private const int ADVICE_ID_LAUNCH = 0;
        private const int ADVICE_ID_CARD_PLACED = 1;
        private const int ADVICE_ID_CARD_INSPECT = 4;
        private const int ADVICE_ID_PLAYER_MOVED = 2;
        private const int ADVICE_ID_THIRD_PHASE_STARTED = 3;

        [Header("Advices")]
        [SerializeField] private DialogueFlowSO _Advices = null;
        [Space(2)]
        [SerializeField] private GameObject _AdviceScreen = null;
        [SerializeField] private TextMeshProUGUI _AdviceText = null;

        [Header("Hand Flow")]
        [SerializeField] private Animator _HandGameobject = null;

        [Space(5)]
        [SerializeField] private string _HandPointingAnimation = "";
        [SerializeField] private string _HandGrabbingAnimation = "";
        [SerializeField] private string _HandRequestMoveAnimation = "";

        // Variables
        private TutorialManager _TutorialManager = null;
        private DialogueManager _DialogueManager = null;

        private bool _FirstMoved = false;

        private bool _FirstCardPlaced = false;

        private void Start()
        {
            _AdviceScreen.SetActive(false);
            _DialogueManager = DialogueManager.GetInstance();

            _TutorialManager = TutorialManager.GetInstance();
            _TutorialManager.OnDialogueEnded += FirstAdvice;

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
            else
            {
                _AdviceScreen.SetActive(false);
            }
        }

        private void ThirdAdvice()
        {
            if (!_FirstMoved)
            {
                Destroy(_HandGameobject.gameObject);

                CreateAdvice(ADVICE_ID_PLAYER_MOVED);
                _FirstMoved = true;

                TEMPCard.OnFocus += AdviceHideWhenCardSelected;
            }    
        }

        public void FourthAdvice()
        {
            CreateAdvice(ADVICE_ID_THIRD_PHASE_STARTED);
            TEMPCard.OnFocus += AdviceHideWhenCardSelected;
        }

        private void AdviceHideWhenCardSelected(bool pState)
        {
            TEMPCard.OnFocus -= AdviceHideWhenCardSelected;
            _AdviceScreen.SetActive(false);
        }

        private void CreateAdvice(int pID)
        {
            _AdviceScreen.SetActive(true);
            _AdviceText.text = _DialogueManager.GetDialogue(_Advices.Dialogues[pID]);
        }

        private void OnDestroy()
        {
            if(_TutorialManager != null)
                _TutorialManager.OnDialogueEnded += FirstAdvice;
            _TutorialManager = null;

            TEMPCard.OnFocus -= VisualHandAdvice;

            GameManager.CardPlaced.RemoveListener(SecondAdvice);
            GameManager.PlayerMoved.RemoveListener(ThirdAdvice);
        }
    }
}
