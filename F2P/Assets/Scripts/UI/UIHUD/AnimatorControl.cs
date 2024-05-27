using System;
using UnityEngine;

namespace Com.IsartDigital.F2P.UI.UIHUD
{
    public class AnimatorControl : MonoBehaviour
    {
        [Header("Trigger Name")]
        [SerializeField] private string _Play;

        private Animator _Animator;

        public void PlayAnimation()
        {
            if (_Animator != null)
            {
                _Animator.SetTrigger(_Play);
            }
        }

    }
}
