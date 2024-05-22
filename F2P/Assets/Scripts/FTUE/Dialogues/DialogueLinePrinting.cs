using System;
using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FTUE.Dialogues
{
    public class DialogueLinePrinting : DialogueScreen
    {
        [Serializable]
        public struct StoryPage
        {
            public Sprite image;
            public string id;
        }

        [SerializeField] private TextMeshProUGUI _DisplayContinue = null;
        [SerializeField] private Image _DisplayImage = null;

        [Header("Flow")]
        [SerializeField] private StoryPage[] _Pages = new StoryPage[0];

        [Space(2)]
        [SerializeField][Range(.01f, 1f)] private float _TimeBeforeSwitchingPage = 0.5f;

        // Variables
        private int _CurrentLineID = 0;
        private string _CurrentLine = "";

        // Events
        [HideInInspector]
        public UnityEvent OnDialogueEnded;

        protected override void Start()
        {
            base.Start();
            int lLength = _Pages.Length;
            m_DialogueIDs = new string[lLength];
            for (int i = 0; i < lLength; i++)
                m_DialogueIDs[i] = _Pages[i].id;

            if (m_DialogueIDs.Length > 0)
                DisplayText();

            _DisplayContinue.gameObject.SetActive(false);
        }

        protected override IEnumerator WriteDialogue()
        {
            m_DialogueManager.OnScreenTouched += SkipTextPrinting;

            int lLength = m_DialogueIDs.Length;
            while (_CurrentLineID <= m_DialogueIDs.Length - 1)
            {
                m_LabelUIText.text = "";

                _DisplayImage.sprite = _Pages[_CurrentLineID].image;
                _CurrentLine = m_DialogueManager.GetDialogue(_Pages[_CurrentLineID].id);

                yield return WriteLine();
                yield return new WaitForSeconds(_TimeBeforeSwitchingPage);
                
                _CurrentLineID += 1;
            }

            StopCoroutine(m_DialogueWriter);

            _DisplayContinue.gameObject.SetActive(true);

            m_DialogueManager.OnScreenTouched -= SkipTextPrinting;
            m_DialogueManager.OnScreenTouched += WaitForContinue;
        }

        private IEnumerator WriteLine()
        {
            int lLength = _CurrentLine.Length;
            float t = m_DisplayDuration / lLength;
            for (int i = 0; i < lLength; i++)
            {
                yield return new WaitForSeconds(t);
                m_LabelUIText.text += _CurrentLine[i];
            }
        }

        private void SkipTextPrinting()
        {
            StopCoroutine(WriteLine());

            // End dialogue printing
            if (_CurrentLineID == _Pages.Length)
            {
                StopAllCoroutines();

                _DisplayContinue.gameObject.SetActive(true);

                m_DialogueManager.OnScreenTouched -= SkipTextPrinting;
                m_DialogueManager.OnScreenTouched += WaitForContinue;
            }
        }

        private void WaitForContinue()
        {
            OnDialogueEnded?.Invoke();
            Destroy(gameObject);
        }

        protected override void OnDestroy()
        {
            if(m_DialogueManager != null)
            {
                m_DialogueManager.OnScreenTouched -= SkipTextPrinting;
                m_DialogueManager.OnScreenTouched -= WaitForContinue;
            }

            OnDialogueEnded.RemoveAllListeners();
            base.OnDestroy();
        }
    }
}
