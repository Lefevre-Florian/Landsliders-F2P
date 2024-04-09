using com.isartdigital.f2p.gameplay.manager;

using System;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    [RequireComponent(typeof(BiomeSurrondingAnalysis))]
    public class Desert : Biome
    {
        private const float MAX = 100f;

        [Header("Design - Settings")]
        [SerializeField][Range(0f, MAX)] private float _TransformationChance = 50f;

        // Variables
        protected override void Start()
        {
            base.Start();
            /// TODO Connect to event turn
        }

        /// <summary>
        /// Determine chance to change one of the surrounding biomes into a desert
        /// </summary>
        private void RollChance()
        {
            float lChance = UnityEngine.Random.Range(0f, MAX);
            if(lChance <= _TransformationChance)
            {
                // Change one the surrounding biomes into a desert

            }
        }

        protected override void PlayEffect(int pGameCurrentPriority)
        {
            base.PlayEffect(pGameCurrentPriority);
            RollChance();
        }

        protected override void OnDestroy()
        {
            /// TODO : Disconnect event turn
            base.OnDestroy();
        }

    }
}
