using System.Collections;

using TMPro;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FTUE.Dialogues
{
    public class DialogueScreen : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private RectTransform _DialogueBox = null;
        [SerializeField] private TextMeshProUGUI _LabelUIText = null;

        [Header("Dialogue flow")]
        [SerializeField] private string _CharacterIDs = "";
        [SerializeField] private string[] _DialogueIDs = new string[0];

        [Header("Juiciness")]
        [SerializeField][Min(1f)] private float _DisplayDuration = 1f;

        // Variables
        private DialogueManager _DialogueManager = null;

        private int _DialogueIdx = 0;
        private Coroutine _DialogueWriter = null;

        private int _TextLength = 0;

        private void Start()
        {
            _DialogueManager = DialogueManager.GetInstance();
            _DialogueManager.OnScreenTouched += Next;

            if(_DialogueIDs.Length > 0)
                DisplayText();
        }

        public void SetDialogues(string pCharacter, string[] pLines)
        {
            _CharacterIDs = pCharacter;
            _DialogueIDs = pLines;
        }

        private void DisplayText()
        {
            if (_DialogueWriter != null)
                StopCoroutine(_DialogueWriter);

            _DialogueWriter = StartCoroutine(WriteDialogue());
        }

        private IEnumerator WriteDialogue()
        {
            string lCurrent = _DialogueManager.GetDialogue(_CharacterIDs, _DialogueIDs[_DialogueIdx]);
            float lPromptTime = _DisplayDuration / lCurrent.Length;

            _TextLength = lCurrent.Length;
            _LabelUIText.maxVisibleCharacters = 0;
            _LabelUIText.text = lCurrent;

            for (int i = 0; i < _TextLength; i++)
            {
                _LabelUIText.maxVisibleCharacters += 1;
                yield return new WaitForSeconds(lPromptTime);
            }

            StopCoroutine(_DialogueWriter);
        }

        private void Next()
        {
            if (_LabelUIText.maxVisibleCharacters != _TextLength)
                return;

            // Skip to next dialogue
            if (_DialogueIdx == _DialogueIDs.Length - 1)
            {
                _DialogueManager.EndDialogue();
                Destroy(gameObject);
                return;
            }
                
            _DialogueIdx += 1;
            DisplayText();
        }

        private void HideBox() => _DialogueBox.gameObject.SetActive(false);

        private void ShowBox() => _DialogueBox.gameObject.SetActive(true);

        private void OnDestroy()
        {
            if(_DialogueManager != null)
                _DialogueManager.OnScreenTouched -= Next;
            _DialogueManager = null;

            StopAllCoroutines();
        }

    }
}
