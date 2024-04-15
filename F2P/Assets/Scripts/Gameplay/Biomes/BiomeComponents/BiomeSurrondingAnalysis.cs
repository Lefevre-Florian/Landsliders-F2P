using com.isartdigital.f2p.gameplay.manager;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    [RequireComponent(typeof(Biome))]
    public class BiomeSurrondingAnalysis : MonoBehaviour
    {
        private enum GridAngle
        {
            AllDirection = 8,
            CrossDirection = 4,
            NextDirection = 2
        }

        [Header("Design")]
        [SerializeField] private GridAngle _GridDirection = GridAngle.CrossDirection;
        [SerializeField][Min(1)] private int _Range = 1;

        // Variables
        protected GridManager m_GridManager = null;
        protected Biome m_Biome = null;

        // Get / Set
        public int NbDirectionToCheck { get { return (int)_GridDirection; } }

        protected virtual void Start()
        {
            m_GridManager = GridManager.GetInstance();
            m_Biome = GetComponent<Biome>();

            // Clamp to max grid size
            if (_Range > m_GridManager._GridSize.x && _Range > m_GridManager._GridSize.y)
                _Range = (int)m_GridManager._GridSize.x;
        }

        /// <summary>
        /// Get Surrounding biome without filter check
        /// </summary>
        /// <returns>Array[Biome] => All neighbour biomes</returns>
        protected Biome[] GetSurrounding()
        {
            List<Biome> lSurroundingBiomes = new List<Biome>();
            Biome lBiome = null;

            Vector2 lSamplePosition = default;

            int lLength = (int)_GridDirection;
            float lAngle = ((Mathf.PI * 2f) / (int)_GridDirection) * Mathf.Rad2Deg;

            for (int i = 0; i < lLength; i++)
            {
                lSamplePosition = m_Biome.GridPosition + (Vector2)(Quaternion.AngleAxis(lAngle * i, Vector3.forward * _Range) * Vector3.up);
                lSamplePosition.x = Mathf.RoundToInt(lSamplePosition.x);
                lSamplePosition.y = Mathf.RoundToInt(lSamplePosition.y);

                // Check out of bound
                if (lSamplePosition.x >= 0f
                    && lSamplePosition.x < m_GridManager._NumCard.x
                    && lSamplePosition.y >= 0f
                    && lSamplePosition.y < m_GridManager._NumCard.y)
                {
                    lBiome = m_GridManager.GetCardByGridCoordinate(lSamplePosition);
                    // Check next card is a biome (avoid crash)
                    if (lBiome != null)
                        lSurroundingBiomes.Add(lBiome);
                }
                    
            }
            return lSurroundingBiomes.ToArray();
        }

        protected Biome[] GetSurroundingRemoveFiltered(BiomeType[] pFilter)
        {
            Biome[] lSurroundingBiomes = GetSurrounding();

            int lLength = lSurroundingBiomes.Length;
            if (lLength == 0)
                return lSurroundingBiomes;

            lLength = pFilter.Length;
            for (int i = 0; i < lLength; i++)
                lSurroundingBiomes.ToList().RemoveAll(x => x.Type == pFilter[i]);

            return lSurroundingBiomes.ToArray();
        }

        protected Biome[] GetSurroundingOnlyFiltered(BiomeType[] pFiler)
        {
            List<BiomeType> lExcludeFilter = new List<BiomeType>();
            foreach (BiomeType lItem in Enum.GetValues(typeof(BiomeType)))
            {
                if (!pFiler.Contains(lItem))
                    lExcludeFilter.Add(lItem);
            }

            return GetSurroundingRemoveFiltered(lExcludeFilter.ToArray());
        }

        protected virtual void OnDestroy() => m_GridManager = null;
    }
}
