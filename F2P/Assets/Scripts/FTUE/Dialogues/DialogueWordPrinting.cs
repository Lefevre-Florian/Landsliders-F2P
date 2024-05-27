using FMOD.Studio;
using FMODUnity;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FTUE.Dialogues
{
    public class DialogueWordPrinting : DialogueScreen
    {
        private const string FMOD_PARAM = "FTE PHASES";

        [Header("Juiciness")]
        [SerializeField][Min(0.5f)] private float _AnimationDuration = 1f;

        [Space(2)]
        [SerializeField] private RectTransform _DialogueBox = null;
        [SerializeField] private RectTransform _CharacterRenderer = null;

        public enum Animation
        {
            NONE,
            RIGHT,
            LEFT,
            DOWN
        }

        // Variables
        private int _DialogueIdx = 0;

        private bool _PrintFinished = false;

        private Animation _Type = Animation.NONE;

        private Vector3 _InitialPosition = default;

        private Dialogue[] _Dialogues = new Dialogue[0];
        private EventInstance _AudioInstance = default;

        // Event
        public UnityEvent OnDialogueEnded;
        public UnityEvent OnDialogueStarted;
        public UnityEvent OnDialoguePaused;

        protected override void Start()
        {
            base.Start();

            _InitialPosition = _DialogueBox.position;

            m_DialogueManager.OnScreenTouched += Next;

            GameFlowManager.Paused?.Invoke();

            if (_Type == Animation.NONE)
                DisplayText();
            else
                PlayAnimation();
        }

        public void SetDialogues(DialogueFlowSO pLineIDs, Animation pAnimationType = Animation.NONE)
        {
            _Dialogues = pLineIDs.Dialogues;
            _Type = pAnimationType;
        }

        protected override IEnumerator WriteDialogue()
        {
            OnDialogueStarted?.Invoke();

            _PrintFinished = false;

            string lCurrent = m_DialogueManager.GetDialogue(_Dialogues[_DialogueIdx].ID);
            float lPromptTime = _Dialogues[_DialogueIdx].Voicing.Duration / lCurrent.Length;

            int lLength = lCurrent.Length;
            m_LabelUIText.maxVisibleCharacters = 0;
            m_LabelUIText.text = lCurrent;

            _AudioInstance = RuntimeManager.CreateInstance(_Dialogues[_DialogueIdx].Voicing.Audio);
            _AudioInstance.setParameterByName(FMOD_PARAM, _Dialogues[_DialogueIdx].Voicing.FMODKey);
            _AudioInstance.start();

            for (int i = 0; i < lLength; i++)
            {
                m_LabelUIText.maxVisibleCharacters += 1;
                yield return new WaitForSeconds(lPromptTime);
            }

            if (_AudioInstance.isValid())
            {
                _AudioInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                _AudioInstance.release();
            }

            OnDialoguePaused?.Invoke();

            _PrintFinished = true;
            StopCoroutine(m_DialogueWriter);
        }

        private void PlayAnimation() => m_DialogueWriter = StartCoroutine(UpdateAnimation());

        private IEnumerator UpdateAnimation()
        {
            Vector3 lEndPosition = _DialogueBox.position;

            Vector3 lDirection = Vector3.zero;
            switch (_Type)
            {
                case Animation.RIGHT:
                    lDirection = Vector3.right;
                    break;
                case Animation.LEFT:
                    lDirection = Vector3.left;
                    break;
                case Animation.DOWN:
                    lDirection = Vector3.down;
                    break;
            }

            Vector3 lStartPosition = lEndPosition + lDirection * _DialogueBox.rect.height;

            _DialogueBox.position = lStartPosition;

            float t = 0;
            while (t < 1)
            {
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
                _DialogueBox.position = Vector3.Lerp(lStartPosition, lEndPosition, t);
            }

            // Clean
            StopCoroutine(m_DialogueWriter);
            m_DialogueWriter = null;

            DisplayText();
        }

        private void Next()
        {
            if (!_PrintFinished)
            {
                _PrintFinished = true;

                if(_AudioInstance.isValid())
                {
                    _AudioInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    _AudioInstance.release();
                }

                if (m_DialogueWriter != null)
                    StopAllCoroutines();
                m_DialogueWriter = null;

                _DialogueBox.position = _InitialPosition;
                m_LabelUIText.maxVisibleCharacters = m_DialogueManager.GetDialogue(_Dialogues[_DialogueIdx].ID).Length;

                return;
            }
                

            // Skip to next dialogue
            if (_DialogueIdx == _Dialogues.Length - 1)
            {
                OnDialogueEnded?.Invoke();
                GameFlowManager.Resumed?.Invoke();

                Destroy(gameObject);
                return;
            }

            _DialogueIdx += 1;
            DisplayText();
        }

        protected override void OnDestroy()
        {
            if (m_DialogueManager != null)
                m_DialogueManager.OnScreenTouched -= Next;

            OnDialogueEnded.RemoveAllListeners();
            OnDialoguePaused.RemoveAllListeners();
            OnDialogueStarted.RemoveAllListeners();

            if (_AudioInstance.isValid())
            {
                _AudioInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                _AudioInstance.release();
            }

            base.OnDestroy();
        }
    }
}
