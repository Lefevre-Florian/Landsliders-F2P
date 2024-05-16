using System;
using System.Collections.Generic;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FTUE
{
    public class DialogueManager : MonoBehaviour
    {
        #region Singleton
        private static DialogueManager _Instance = null;

        public static DialogueManager GetInstance()
        {
            if (_Instance == null)
                _Instance = new DialogueManager();
            return _Instance;
        }

        private DialogueManager() : base() { }
        #endregion

        private const char CSV_SEPARATOR = ';';
        private const char LINE_SEPARATOR = '\n';

        private const int ID_DIALOGUE_COLUMN = 0;
        private const int DIALOGUE_CONTENT_COLUMN = 1;

        [Header("File")]
        [SerializeField] private TextAsset _DialogueCSV = null;

        // Variables
        private Dictionary<string, string> _Dialogues = null;

        // Event
        public event Action OnDialogueStarted;
        public event Action OnDialogueEnded;

        public event Action OnScreenTouched;

        private void Awake()
        {
            if (_Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _Instance = this;

            // Read file
            if (_DialogueCSV == null)
                return;

            string[] lContent = _DialogueCSV.text.Split(LINE_SEPARATOR);

            string[] lCSVLine = null;

            int lLength = lContent.Length;
            if (lLength <= 1)
                return;


            _Dialogues = new Dictionary<string, string>();

            for (int i = 1; i < lLength; i++)
            {
                lCSVLine = lContent[i].Split(CSV_SEPARATOR);

                if (lCSVLine.Length <= 1)
                    break;

                _Dialogues.Add(lCSVLine[ID_DIALOGUE_COLUMN],
                               lCSVLine[DIALOGUE_CONTENT_COLUMN]);

            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                OnScreenTouched?.Invoke();
        }

        public string GetDialogue(string pDialogueID)
        {
            if (_Dialogues == null || !_Dialogues.ContainsKey(pDialogueID))
                return "";

            return _Dialogues[pDialogueID];
        }

        public void TriggerDialogue() => OnDialogueStarted?.Invoke();

        public void EndDialogue() => OnDialogueEnded?.Invoke();

        private void OnDestroy()
        {
            if(_Instance == this)
            {
                _Instance = null;
            }
        }

    }
}
