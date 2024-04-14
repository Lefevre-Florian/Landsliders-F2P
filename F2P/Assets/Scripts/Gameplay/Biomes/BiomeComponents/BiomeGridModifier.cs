using com.isartdigital.f2p.gameplay.manager;
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

// Author (CR): Lefevre Florian
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
        [SerializeField] private BiomeType[] _BiomeTypeToReplace = Enum.GetValues(typeof(BiomeType))
                                                                       .Cast<BiomeType>()
                                                                       .ToArray();

        [Space(2)]
        [SerializeField] private bool _IsRandomReplace = true;

        // Variables
        private BiomeSurrondingAnalysis _SurroundingComponent = null;

        private GridManager _GridManager = null;

        private void Start()
        {
            _GridManager = GridManager.GetInstance();
            _SurroundingComponent = GetComponent<BiomeSurrondingAnalysis>();
        }

        public void UpdateGrid()
        {
            float lRnd = UnityEngine.Random.Range(MIN, MAX);
            if (lRnd > _ChanceOfModification)
                return;

            List<Biome> lCards;
            if (_BiomeTypeToReplace.Length == Enum.GetValues(typeof(BiomeType)).Length)
                lCards = _SurroundingComponent.GetSurrounding().ToList();
            else
                lCards = _SurroundingComponent.GetSurroundingOnlyFiltered(_BiomeTypeToReplace).ToList();
   
            lCards.RemoveAll(x => !x.CanBeRemoved);

            int lLength = lCards.Count;
            int lRatio = Mathf.RoundToInt((_SurroundingComponent.NbDirectionToCheck) * (_ChangeRatio / 100f));

            Biome lCard = null;
            Transform lTile;
            int lIdx = 0;

            lRatio = (lRatio > lLength) ? lLength : lRatio;
            for (int i = 0; i < lRatio; i++)
            {
                lIdx = UnityEngine.Random.Range(0, lCards.Count - 1);

                lCard = lCards[lIdx].GetComponent<Biome>();
                lTile = lCard.transform.parent;

                lCard.Remove();
                lCards.RemoveAt(lIdx);

                print("Replaced : " + lCard.name);

                if (!_IsRandomReplace)
                    Instantiate(transform, lTile);
                else
                    Instantiate(_GridManager.GetRandomBiome(), lTile);
            }
            print(transform.name);
        }

        private void OnDestroy()
        {
            _GridManager = null;
            _SurroundingComponent = null;
        }

    }
}
