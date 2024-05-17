using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.Events;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FTUE.Dialogues
{
    public class DialogueLinePrinting : DialogueScreen
    {
        [SerializeField] private TextMeshProUGUI _DisplayContinue = null;

        // Variables
        private int _CurrentLineID = 0;

        // Events
        [HideInInspector]
        public UnityEvent OnDialogueEnded;

        protected override void Start()
        {
            base.Start();
            if (m_DialogueIDs.Length > 0)
            {
                DisplayText();
            }

            _DisplayContinue.gameObject.SetActive(false);
        }

        protected override IEnumerator WriteDialogue()
        {
            m_DialogueManager.OnScreenTouched += SkipTextPrinting;

            int lLength = m_DialogueIDs.Length;
            while (_CurrentLineID <= m_DialogueIDs.Length - 1)
            {
                yield return WriteLine(_CurrentLineID);

                _CurrentLineID += 1;
                m_LabelUIText.text += '\n';
            }

            StopCoroutine(m_DialogueWriter);

            _DisplayContinue.gameObject.SetActive(true);

            m_DialogueManager.OnScreenTouched -= SkipTextPrinting;
            m_DialogueManager.OnScreenTouched += WaitForContinue;
        }

        private IEnumerator WriteLine(int pID)
        {
            string lLine = m_DialogueManager.GetDialogue(m_DialogueIDs[pID]);

            int lLength = lLine.Length;
            float t = m_DisplayDuration / lLength;
            for (int i = 0; i < lLength; i++)
            {
                yield return new WaitForSeconds(t);
                m_LabelUIText.text += lLine[i];
            }
        }

        private void SkipTextPrinting()
        {
            StopCoroutine(WriteLine(_CurrentLineID));
            string lLines = "";

            if (_CurrentLineID >= m_DialogueIDs.Length)
                _CurrentLineID = m_DialogueIDs.Length;

            for (int i = 0; i < _CurrentLineID; i++)
            {
                lLines += m_DialogueManager.GetDialogue(m_DialogueIDs[i]);
                lLines += '\n';
            }

            _CurrentLineID += 1;
            m_LabelUIText.text = lLines;
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
