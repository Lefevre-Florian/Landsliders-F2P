using System;

using UnityEngine;

namespace Com.IsartDigital.F2P.Biomes
{
    public class Desert : MonoBehaviour
    {
        private const float MAX = 100f;

        [Header("Design - Settings")]
        [SerializeField][Range(0f, MAX)] private float _TransformationChance = 50f;

        // Variables

        private void Start()
        {
            
        }

        /// <summary>
        /// Determine chance to change one of the surrounding biomes into a desert
        /// </summary>
        private void RollChance()
        {
            float lChance = UnityEngine.Random.Range(0f, MAX);
            if(lChance <= _TransformationChance)
            {
                // Do thing
            }
        }

    }
}
