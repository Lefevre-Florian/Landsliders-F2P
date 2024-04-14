using com.isartdigital.f2p.gameplay.manager;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
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
        private GridManager _GridManager = null;

        private Vector2 _GridPosition = Vector2.zero;

        // Get / Set
        public int NbDirectionToCheck { get { return (int)_GridDirection; } }

        private void Start()
        {
            _GridManager = GridManager.GetInstance();
            _GridPosition = _GridManager.GetGridCoordinate(transform.position);

            // Clamp to max grid size
            if (_Range > _GridManager._GridSize.x && _Range > _GridManager._GridSize.y)
                _Range = (int)_GridManager._GridSize.x;
        }

        /// <summary>
        /// Get Surrounding biome without filter check
        /// </summary>
        /// <returns>Array[Biome] => All neighbour biomes</returns>
        public Biome[] GetSurrounding()
        {
            List<Biome> lSurroundingBiomes = new List<Biome>();
            Biome lBiome = null;

            Vector2 lSamplePosition = default;

            int lLength = (int)_GridDirection;
            float lAngle = ((Mathf.PI * 2f) / (int)_GridDirection) * Mathf.Rad2Deg;

            for (int i = 0; i < lLength; i++)
            {
                lSamplePosition = _GridPosition + (Vector2)(Quaternion.AngleAxis(lAngle * i, Vector3.forward * _Range) * Vector3.up);
                lSamplePosition.x = Mathf.RoundToInt(lSamplePosition.x);
                lSamplePosition.y = Mathf.RoundToInt(lSamplePosition.y);

                // Check out of bound
                if (lSamplePosition.x >= 0f
                    && lSamplePosition.x < _GridManager._NumCard.x
                    && lSamplePosition.y >= 0f
                    && lSamplePosition.y < _GridManager._NumCard.y)
                {
                    lBiome = _GridManager.GetCardByGridCoordinate(lSamplePosition).GetComponent<Biome>();
                    
                    // Check next card is a biome (avoid crash)
                    if (lBiome != null)
                        lSurroundingBiomes.Add(lBiome);
                }
                    
            }
            return lSurroundingBiomes.ToArray();
        }

        public Biome[] GetSurroundingRemoveFiltered(BiomeType[] pFilter)
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

        public Biome[] GetSurroundingOnlyFiltered(BiomeType[] pFiler)
        {
            List<BiomeType> lExcludeFilter = new List<BiomeType>();
            foreach (BiomeType lItem in Enum.GetValues(typeof(BiomeType)))
            {
                if (!pFiler.Contains(lItem))
                    lExcludeFilter.Add(lItem);
            }

            return GetSurroundingRemoveFiltered(lExcludeFilter.ToArray());
        }

        private void OnDestroy() => _GridManager = null;
    }
}
