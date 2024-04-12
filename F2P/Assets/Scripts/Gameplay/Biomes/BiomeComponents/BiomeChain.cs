using com.isartdigital.f2p.gameplay.manager;

using System;
using System.Collections.Generic;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    [RequireComponent(typeof(BiomeSurrondingAnalysis),
                      typeof(Biome))]
    public class BiomeChain : MonoBehaviour
    {
        [Header("Design")]
        [SerializeField][Min(1)] private int _Range = int.MaxValue;

        // Variables
        private List<Biome> _ShortMemory = null;

        private BiomeType _Type = default;

        private BiomeSurrondingAnalysis _BiomeSurroundingAnalyser = null;

        private void Start()
        {
            _Type = GetComponent<Biome>().Type;
            _BiomeSurroundingAnalyser = GetComponent<BiomeSurrondingAnalysis>();
        }

        public int GetChainLength()
        {
            int lStep = 0;
            _ShortMemory = new List<Biome>
            {
                GetComponent<Biome>()
            };

            int lLength = _BiomeSurroundingAnalyser.NbDirectionToCheck;
            for (int i = 0; i < lLength; i++)
                RecursiveAnalysis(lStep);

            return _ShortMemory.Count - 1;
        }

        private void RecursiveAnalysis(int pStep)
        {
            if (pStep >= _Range)
                return;
            pStep += 1;

            Biome[] lBiomes = _BiomeSurroundingAnalyser.GetSurroundingOnlyFiltered(new BiomeType[] { _Type });
            int lLength = lBiomes.Length;

            for (int i = 0; i < lLength; i++)
            {
                if (!_ShortMemory.Contains(lBiomes[i]))
                {
                    _ShortMemory.Add(lBiomes[i]);
                    RecursiveAnalysis(pStep);
                }
            }

            return;
        }

        private void OnDestroy()
        {
            _ShortMemory.Clear();
            _ShortMemory = null;
        }

    }
}
