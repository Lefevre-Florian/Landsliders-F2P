using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;

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

            int lRatio = Mathf.RoundToInt(NbDirectionToCheck * (_ChangeRatio / 100f));
            lRatio = (lRatio > lCards.Count) ? lCards.Count : lRatio;

            Biome lCard = null;
            int lIdx = 0;

            for (int i = 0; i < lRatio; i++)
            {
                lIdx = UnityEngine.Random.Range(0, lCards.Count - 1);

                lCard = lCards[lIdx].GetComponent<Biome>();
                lCards.RemoveAt(lIdx);

                m_GridManager.ReplaceAtIndex(lCard.GridPosition,(_IsRandomReplace) ? m_GridManager.GetRandomBiome() : _SubstitutionBiome);
            }
        }

        public void UpdateBetweenTypeNeighbour(int pRange = 2)
        {
            float lRnd = UnityEngine.Random.Range(MIN, MAX);
            if (lRnd > _ChanceOfModification)
                return;

            if(pRange <= 1)
            {
                Debug.LogError("The range must be superior or equal at 2");
                return;
            }

            List<Biome> lCards = GetSurroundingOnlyFiltered(new BiomeType[] { m_Biome.Type }, pRange).ToList();
            int lLength = lCards.Count;

            Vector2 lDirection = default;
            Vector2 lNextPosition = default;

            for (int i = 0; i < lLength; i++)
            {
                lDirection = (lCards[i].GridPosition - m_Biome.GridPosition).normalized;
                for (int j = 1; j < pRange; j++)
                {
                    lNextPosition = m_Biome.GridPosition + lDirection * j;
                    Debug.Log("Replace at " + lNextPosition);

                    m_GridManager.ReplaceAtIndex(lNextPosition, (_IsRandomReplace) ? m_GridManager.GetRandomBiome() : _SubstitutionBiome);
                }
            }
        }
    }
}
