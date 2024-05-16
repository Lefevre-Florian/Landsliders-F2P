using System.Collections;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FTUE.Dialogues
{
    public class DialogueLinePrinting : DialogueScreen
    {

        protected override void Start()
        {
            base.Start();
            if (m_DialogueIDs.Length > 0)
                DisplayText();
        }

        protected override IEnumerator WriteDialogue()
        {
            int lLength = m_DialogueIDs.Length;
            string lCurrent = "";

            for (int i = 0; i < lLength; i++)
            {
                yield return new WaitForSeconds(m_DisplayDuration);

                lCurrent = m_DialogueManager.GetDialogue(m_DialogueIDs[i]);
                m_LabelUIText.text += '\n' + lCurrent;

                m_LabelUIText.lineSpacing = m_LabelUIText.lineSpacing;
            }

            yield return new WaitForSeconds(m_DisplayDuration);
            StopCoroutine(m_DialogueWriter);

            Destroy(gameObject);
        }
    }
}
