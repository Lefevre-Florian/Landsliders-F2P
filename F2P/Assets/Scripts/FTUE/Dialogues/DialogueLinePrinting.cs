using FMOD.Studio;
using FMODUnity;

using System;
using System.Collections;

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
            public string audioValue;
        }

        private const string FMOD_PARAMETER = "FTE PHASES";

        [SerializeField] private Image _DisplayImage = null;

        [Header("Flow")]
        [SerializeField] private StoryPage[] _Pages = new StoryPage[0];
        [SerializeField] private EventReference _AudioDialogue = default;

        [Space(2)]
        [SerializeField][Range(.01f, 10f)] private float _TimeBeforeSwitchingPage = 0.5f;

        // Variables
        private int _CurrentLineID = 0;
        private string _CurrentLine = "";

        private bool _Skip = false;

        private EventInstance _DialogueInstance = default;

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
        }

        protected override IEnumerator WriteDialogue()
        {
            m_DialogueManager.OnScreenTouched += SkipTextPrinting;

            int lLineLength = _CurrentLine.Length;
            float t = m_DisplayDuration / lLineLength;

            float lSpeed = 1f / _TimeBeforeSwitchingPage;

            _DialogueInstance = RuntimeManager.CreateInstance(_AudioDialogue);
            _DialogueInstance.start();

            while (_CurrentLineID <= m_DialogueIDs.Length - 1)
            {
                _Skip = false;
                _DialogueInstance.setParameterByName(FMOD_PARAMETER, _CurrentLineID);
                _DialogueInstance.start();

                m_LabelUIText.text = "";

                _DisplayImage.color = new Color(_DisplayImage.color.r, _DisplayImage.color.g, _DisplayImage.color.b, 1f);
                _DisplayImage.sprite = _Pages[_CurrentLineID].image;
                _CurrentLine = m_DialogueManager.GetDialogue(_Pages[_CurrentLineID].id);

                lLineLength = _CurrentLine.Length;
                t = m_DisplayDuration / lLineLength;
                for (int i = 0; i < lLineLength; i++)
                {
                    yield return new WaitForSeconds(t);
                    m_LabelUIText.text += _CurrentLine[i];

                    if (_Skip)
                    {
                        m_LabelUIText.text = "";
                        m_LabelUIText.text = _CurrentLine;

                        break;
                    }
                }

                if (!_Skip && _CurrentLineID < m_DialogueIDs.Length - 1)
                {
                    t = 0;
                    while (t < 1f)
                    {
                        t += Time.deltaTime * lSpeed;
                        _DisplayImage.color = new Color(_DisplayImage.color.r, 
                                                        _DisplayImage.color.g, 
                                                        _DisplayImage.color.b, 
                                                        1f - t);

                        yield return new WaitForEndOfFrame();
                    }
                }

                _CurrentLineID += 1;
            }

            if (_DialogueInstance.isValid())
                _DialogueInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

            StopCoroutine(m_DialogueWriter);
            m_DialogueManager.OnScreenTouched -= SkipTextPrinting;
            m_DialogueManager.OnScreenTouched += WaitForContinue;
        }


        private void SkipTextPrinting()
        {
            if (!_Skip)
            {
                _Skip = true;   
            }

            // End dialogue printing
            if (_CurrentLineID == _Pages.Length)
            {
                StopAllCoroutines();
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

            if (_DialogueInstance.isValid())
                _DialogueInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

            OnDialogueEnded.RemoveAllListeners();
            base.OnDestroy();
        }
    }
}
