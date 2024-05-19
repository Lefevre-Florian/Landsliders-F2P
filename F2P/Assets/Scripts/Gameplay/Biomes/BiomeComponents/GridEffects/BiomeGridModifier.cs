using com.isartdigital.f2p.gameplay.quest;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

// Author (CR): Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    [RequireComponent(typeof(Biome))]
    public class BiomeGridModifier : BiomeSurrondingAnalysis, IBiomeSupplier
    {
        private const float MAX = 100f;
        private const float MIN = 0f;

        private const string ERR_BETWEEN_RANGE = "The range must be superior or equal at 2";

        [Header("Design")]
        [SerializeField][Range(1, 3)] private int _Range = 1;

        [Space(2)]
        [SerializeField][Range(0f, 100f)] private float _ChanceOfModification = 50f;
        [SerializeField][Range(0f, 100f)] private float _ChangeRatio = 100f;
        [SerializeField] private BiomeType[] _BiomeTypeToReplace = Enum.GetValues(typeof(BiomeType))
                                                                       .Cast<BiomeType>()
                                                                       .ToArray();

        [Space(2)]
        [SerializeField] private bool _IsRandomReplace = true;
        [SerializeField] private BiomeType _SubstitutionBiome = default;

        [Space(5)]
        public UnityEvent OnGridModified;

        public void UpdateNeigbourhood()
        {
            List<Biome> lBiomes = GetNeighbourBiomes(_Range);
            if (lBiomes == null || lBiomes.Count == 0)
                return;

            PerformedNeighbourhoodModification(lBiomes.ToArray());
        }

        public void UpdateNeigbourhood(MonoBehaviour pSupplier)
        {
            if(pSupplier is not IBiomeSupplier)
            {
                Debug.LogError("Must be of type" + typeof(IBiomeSupplier));
                return;
            }

            Vector2[] lPositions = (pSupplier as IBiomeSupplier).SupplyBiomes();
            if(lPositions != null && lPositions.Length > 0)
            {
                Biome[] lBiomes = new Biome[lPositions.Length];
                int lLength = lBiomes.Length;

                for (int i = 0; i < lLength; i++)
                    lBiomes[i] = m_GridManager.GetCardByGridCoordinate(lPositions[i]);

                PerformedNeighbourhoodModification(lBiomes);
            }
        }

        [HideInInspector]
        public Vector2[] SupplyBiomes()
        {
            List<Biome> lBiomes = GetNeighbourBiomes(_Range);
            if (lBiomes == null || lBiomes.Count == 0)
                return null;

            int lLength = lBiomes.Count;
            Vector2[] lPositions = new Vector2[lLength];

            for (int i = 0; i < lLength; i++)
                lPositions[i] = lBiomes[i].GridPosition;

            return lPositions;
        }

        public void UpdateBetweenTypeNeighbour()
        {
            float lRnd = UnityEngine.Random.Range(MIN, MAX);
            if (lRnd > _ChanceOfModification)
                return;

            if(_Range <= 1)
            {
                Debug.LogError(ERR_BETWEEN_RANGE);
                return;
            }

            List<Biome> lCards = GetSurroundingOnlyFiltered(new BiomeType[] { m_Biome.Type }, _Range).ToList();
            int lLength = lCards.Count;

            Vector2 lDirection = default;
            Vector2 lNextPosition = default;

            for (int i = 0; i < lLength; i++)
            {
                lDirection = (lCards[i].GridPosition - m_Biome.GridPosition).normalized;
                for (int j = 1; j < _Range; j++)
                {
                    lNextPosition = m_Biome.GridPosition + lDirection * j;
                    
                    if(lNextPosition.x % 1f != 0f && lNextPosition.y % 1f != 0f)
                    {
                        lNextPosition.x = lNextPosition.x % 1 <= 0.5f ? Mathf.FloorToInt(lNextPosition.x) : Mathf.CeilToInt(lNextPosition.x);
                        lNextPosition.y = lNextPosition.y % 1 <= 0.5f ? Mathf.FloorToInt(lNextPosition.y) : Mathf.CeilToInt(lNextPosition.y);

                        lNextPosition.x = lNextPosition.x + (Mathf.Sign(lDirection.x) * (j / 2));
                        lNextPosition.y = lNextPosition.y + (Mathf.Sign(lDirection.y) * (j / 2));
                    }

                    m_GridManager.ReplaceAtIndex(lNextPosition, (_IsRandomReplace) ? CardPrefabDic.GetRandomPrefab().transform 
                                                                                   : CardPrefabDic.GetPrefab(_SubstitutionBiome).transform);
                    
                    if (TryGetComponent<VortexQuest>(out VortexQuest vq)) vq.ValidQuest(lNextPosition);
                }
            }

            OnGridModified?.Invoke();
        }

        private List<Biome> GetNeighbourBiomes(int pRange)
        {
            float lRnd = UnityEngine.Random.Range(MIN, MAX);
            if (lRnd > _ChanceOfModification)
                return null;

            List<Biome> lCards;
            if (_BiomeTypeToReplace.Length == Enum.GetValues(typeof(BiomeType)).Length)
                lCards = GetSurrounding(pRange).ToList();
            else
                lCards = GetSurroundingOnlyFiltered(_BiomeTypeToReplace, pRange).ToList();

            lCards.RemoveAll(x => !x.CanBeReplaced);

            int lRatio = Mathf.RoundToInt(NbDirectionToCheck * (_ChangeRatio / 100f));
            lRatio = (lRatio > lCards.Count) ? lCards.Count : lRatio;

            List<Biome> lBiomes = new List<Biome>();

            int lIdx = 0;
            for (int i = 0; i < lRatio; i++)
            {
                lIdx = UnityEngine.Random.Range(0, lCards.Count);
                lBiomes.Add(lCards[lIdx]);
                lCards.RemoveAt(lIdx);
            }

            return lBiomes;
        }

        private void PerformedNeighbourhoodModification(Biome[] pBiomes)
        {
            OnGridModified?.Invoke();

            int lLength = pBiomes.Length;
            for (int i = 0; i < lLength; i++)
                m_GridManager.ReplaceAtIndex(pBiomes[i].GridPosition, (_IsRandomReplace) ? CardPrefabDic.GetRandomPrefab().transform : CardPrefabDic.GetPrefab(_SubstitutionBiome).transform);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnGridModified.RemoveAllListeners();
        }
    }
}
