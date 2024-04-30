using System.Collections;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Cards
{
    public class CardRenderer : MonoBehaviour
    {
        private const float FLIP_ANGLE = 180f;

        [Header("Animation")]
        [SerializeField] private string _AnimationTitle = "Animated";

        [Header("Juiciness")]
        [SerializeField] private float _FlipAnimationSpeed = 0.25f;

        // Variables
        private Animator _Animator = null;

        private Coroutine _Tween = null;

        private void Start()
        {
            _Animator = GetComponent<Animator>();

            DisableAnimation();
        }


        public void EnableAnimation()
        {
            _Animator.enabled = true;
            _Animator.Play(_AnimationTitle);
        }

        public void DisableAnimation()
        {
            _Animator.enabled = false;
        }

        public void FlipCard()
        {
            if (_Animator.enabled)
                DisableAnimation();
            else
                EnableAnimation();

            _Tween = StartCoroutine(FlipAnimation());
        }

        /// ------------------------ ///
        /// Coroutine for the moment ///
        /// ------------------------ ///

        private IEnumerator FlipAnimation()
        {
            Quaternion lOrigin = transform.rotation;
            Quaternion lTarget = Quaternion.AngleAxis(FLIP_ANGLE, Vector3.up) * transform.rotation;

            float t = 0;

            while (t < 1f)
            {
                transform.rotation = Quaternion.Slerp(lOrigin, lTarget, t);
                t += Time.deltaTime * _FlipAnimationSpeed;

                yield return new WaitForEndOfFrame();
            }

            if(_Tween != null)
            {
                StopCoroutine(_Tween);
                _Tween = null;
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            _Tween = null;
        }
    }
}
