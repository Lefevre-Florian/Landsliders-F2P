using System.Collections;

using UnityEngine;
using UnityEngine.Events;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FTUE.Dialogues
{
    public class DialogueWordPrinting : DialogueScreen
    {
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

        // Event
        public UnityEvent OnDialogueEnded;
        public UnityEvent OnDialogueStarted;
        public UnityEvent OnDialoguePaused;

        protected override void Start()
        {
            base.Start();
            m_DialogueManager.OnScreenTouched += Next;

            if (_Type == Animation.NONE)
                DisplayText();
            else
                PlayAnimation();
        }

        public void SetDialogues(string[] pLineIDs, Animation pAnimationType = Animation.NONE, bool pDisplayIcon = true)
        {
            m_DialogueIDs = pLineIDs;
            _Type = pAnimationType;

            if (!pDisplayIcon)
                _CharacterRenderer.gameObject.SetActive(false);
        }

        protected override IEnumerator WriteDialogue()
        {
            OnDialogueStarted?.Invoke();

            _PrintFinished = false;

            string lCurrent = m_DialogueManager.GetDialogue(m_DialogueIDs[_DialogueIdx]);
            float lPromptTime = m_DisplayDuration / lCurrent.Length;

            int lLength = lCurrent.Length;
            m_LabelUIText.maxVisibleCharacters = 0;
            m_LabelUIText.text = lCurrent;

            for (int i = 0; i < lLength; i++)
            {
                m_LabelUIText.maxVisibleCharacters += 1;
                yield return new WaitForSeconds(lPromptTime);
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
                return;

            // Skip to next dialogue
            if (_DialogueIdx == m_DialogueIDs.Length - 1)
            {
                OnDialogueEnded?.Invoke();
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

            base.OnDestroy();
        }
    }
}
