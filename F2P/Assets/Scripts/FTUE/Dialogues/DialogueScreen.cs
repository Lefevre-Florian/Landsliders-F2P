using System.Collections;

using TMPro;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FTUE.Dialogues
{
    public abstract class DialogueScreen : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] protected TextMeshProUGUI m_LabelUIText = null;

        [Header("Juiciness")]
        [SerializeField][Min(0f)] protected float m_DisplayDuration = 1f;

        // Variables
        protected DialogueManager m_DialogueManager = null;
        protected Coroutine m_DialogueWriter = null;

        protected string[] m_DialogueIDs = new string[0];

        protected virtual void Start()
        {   
            m_LabelUIText.text = "";
            m_DialogueManager = DialogueManager.GetInstance();
        }

        protected void DisplayText()
        {
            if (m_DialogueWriter != null)
                StopCoroutine(m_DialogueWriter);

            m_DialogueWriter = StartCoroutine(WriteDialogue());
        }

        protected virtual IEnumerator WriteDialogue() { yield return null; }

        protected virtual void OnDestroy()
        {
            m_DialogueManager = null;
            StopAllCoroutines();
        }
    }
}
