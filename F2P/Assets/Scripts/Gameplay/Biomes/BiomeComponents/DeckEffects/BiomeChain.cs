using System.Collections.Generic;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    [RequireComponent(typeof(Biome))]
    public class BiomeChain : BiomeSurrondingAnalysis, IBiomeEnumerator
    {
        [Header("Design")]
        [SerializeField][Min(1)] private int _ChainMaxLength = int.MaxValue;

        // Variables
        private List<Biome> _ShortMemory = null;

        private BiomeType _Type = default;

        protected override void Start()
        {
            base.Start();
            _Type = GetComponent<Biome>().Type;
        }

        public int GetEnumertation() => GetChainLength();

        private int GetChainLength()
        {
            int lStep = 0;
            _ShortMemory = new List<Biome>
            {
                GetComponent<Biome>()
            };

            int lLength = NbDirectionToCheck;
            for (int i = 0; i < lLength; i++)
                RecursiveAnalysis(lStep, this);
            return _ShortMemory.Count - 1;
        }

        private void RecursiveAnalysis(int pStep, BiomeChain pChain)
        {
            if (pStep >= _ChainMaxLength)
                return;
            pStep += 1;

            Biome[] lBiomes = pChain.GetSurroundingOnlyFiltered(new BiomeType[] { _Type }, 1);
            int lLength = lBiomes.Length;
            
            if (lLength == 0)
                return;

            for (int i = 0; i < lLength; i++)
            {
                if (!_ShortMemory.Contains(lBiomes[i]))
                {
                    _ShortMemory.Add(lBiomes[i]);
                    RecursiveAnalysis(pStep, lBiomes[i].GetComponent<BiomeChain>());
                }
            }
        }

        protected override void OnDestroy()
        {
            if(_ShortMemory != null && _ShortMemory.Count > 0)
            {
                _ShortMemory.Clear();
                _ShortMemory = null;
            }

            base.OnDestroy();
        }
    }
}
