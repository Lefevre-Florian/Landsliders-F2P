using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI.Screens
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField][Min(0.1f)] private float _Duration = 2f;
        [SerializeField] private AnimationCurve _Flow = null;

        [Space(5)]
        [SerializeField] private Image _HidePanel = null;

        // Variables
        private Coroutine _Animator = null;

        private void Start()
        {
            _HidePanel.color = new Color(_HidePanel.color.r, _HidePanel.color.g, _HidePanel.color.b, 1f);
            _Animator = StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            float lElapsedTime = 0f;
            while (lElapsedTime <= _Duration)
            {
                lElapsedTime += Time.deltaTime;
                _HidePanel.color = new Color(_HidePanel.color.r,
                                             _HidePanel.color.g,
                                             _HidePanel.color.b,
                                             _Flow.Evaluate(lElapsedTime / _Duration));
                yield return new WaitForEndOfFrame();
            }

            Clear();
        }

        private void Clear()
        {
            if (_Animator != null)
                StopCoroutine(_Animator);
            _Animator = null;
        }

        private void OnDestroy() => Clear();
    }
}
