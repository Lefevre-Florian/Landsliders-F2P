using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

// Author (CR): Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    [RequireComponent(typeof(Biome))]
    public class BiomeGridModifier : BiomeSurrondingAnalysis
    {
        private const float MAX = 100f;
        private const float MIN = 0f;

        [Header("Design")]
        [SerializeField][Range(0f, 100f)] private float _ChanceOfModification = 50f;
        [SerializeField][Range(0f, 100f)] private float _ChangeRatio = 100f;
        [SerializeField] private BiomeType[] _BiomeTypeToReplace = Enum.GetValues(typeof(BiomeType))
                                                                       .Cast<BiomeType>()
                                                                       .ToArray();

        [Space(2)]
        [SerializeField] private bool _IsRandomReplace = true;
        [SerializeField] private Transform _SubstitutionBiome = null;

        public void UpdateNeigbourhood(int pRange)
        {
            float lRnd = UnityEngine.Random.Range(MIN, MAX);
            if (lRnd > _ChanceOfModification)
                return;

            List<Biome> lCards;
            if (_BiomeTypeToReplace.Length == Enum.GetValues(typeof(BiomeType)).Length)
                lCards = GetSurrounding(pRange).ToList();
            else
                lCards = GetSurroundingOnlyFiltered(_BiomeTypeToReplace, pRange).ToList();
   
            lCards.RemoveAll(x => !x.CanBeReplaced);

            int lLength = lCards.Count;
            int lRatio = Mathf.RoundToInt(NbDirectionToCheck * (_ChangeRatio / 100f));

            Biome lCard = null;
            int lIdx = 0;

            lRatio = (lRatio > lLength) ? lLength : lRatio;
            for (int i = 0; i < lRatio; i++)
            {
                lIdx = UnityEngine.Random.Range(0, lCards.Count - 1);

                lCard = lCards[lIdx].GetComponent<Biome>();
                lCards.RemoveAt(lIdx);

                if (!_IsRandomReplace)
                    m_GridManager.ReplaceAtIndex(lCard.GridPosition, _SubstitutionBiome);
                else
                    m_GridManager.ReplaceAtIndex(lCard.GridPosition, m_GridManager.GetRandomBiome());
            }
        }
    }
}
