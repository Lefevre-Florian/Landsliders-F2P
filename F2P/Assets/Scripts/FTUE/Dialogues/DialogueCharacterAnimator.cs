using UnityEngine;
using UnityEngine.UI;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FTUE.Dialogues
{
    public class DialogueCharacterAnimator : MonoBehaviour
    {
        [Header("Renderer")]
        [SerializeField] private Image _Renderer = null;

        [Header("Settings")]
        [SerializeField][Min(0.25f)] private float _LoopDuration = 1f;
        [SerializeField] private Sprite[] _Frames = new Sprite[0];

        // Variables
        private int _CurrentFrame = 0;

        private bool _Playing = false;

        private float _ElapsedTime = 0f;
        private float _StepDuration = 0f;

        private void Start()
        {
            if (_Frames.Length == 0)
                Destroy(this);

            _StepDuration = _LoopDuration / _Frames.Length;

            _Renderer.sprite = _Frames[0];
        }

        private void Update()
        {
            if (!_Playing)
                return;

            _ElapsedTime += Time.deltaTime;
            if(_ElapsedTime >= _StepDuration)
            {
                _ElapsedTime = 0f;
                _CurrentFrame = (_CurrentFrame + 1 >= _Frames.Length) ? 0 : _CurrentFrame + 1;

                _Renderer.sprite = _Frames[_CurrentFrame];
            }
        }

        public void StopAnimation()
        {
            _Playing = false;
            _Renderer.sprite = _Frames[0];
        }

        public void StartAnimation()
        {
            _Playing = true;
            _Renderer.sprite = _Frames[0];
        }
    }
}
