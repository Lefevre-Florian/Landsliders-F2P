using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Com.IsartDigital.F2P.Biomes
{
    [RequireComponent(typeof(BiomeSurrondingAnalysis))]
    public class BiomeGridModifier : MonoBehaviour
    {
        private const float MAX = 100f;
        private const float MIN = 0f;

        [Header("Design")]
        [SerializeField][Range(0f, 100f)] private float _ChanceOfModification = 50f;
        [SerializeField][Range(0f, 100f)] private float _ChangeRatio = 100f;
        [SerializeField] private BiomeType _Biome;
        [SerializeField] private bool _IsReplacementForced = true;

        // Variables
        private BiomeSurrondingAnalysis _SurroundingComponent = null;

        private void Start()
        {
            _SurroundingComponent = GetComponent<BiomeSurrondingAnalysis>();
        }

        public void UpdateGrid()
        {
            float lRnd = UnityEngine.Random.Range(MIN, MAX);
            if (lRnd > _ChanceOfModification)
                return;

            List<GameObject> lCards = _SurroundingComponent.GetSurrounding()
                                                           .ToList<GameObject>();
            int lLength = lCards.Count;
            int lRatio = Mathf.RoundToInt(lLength * _ChangeRatio / 100f);
            int lIdx = 0;

            for (int i = 0; i < lRatio; i++)
            {
                lIdx = UnityEngine.Random.Range(0, lCards.Count - 1);
                lCards.RemoveAt(lIdx);
            }
        }

        private void OnDestroy()
        {
            _SurroundingComponent = null;
        }

    }
}
