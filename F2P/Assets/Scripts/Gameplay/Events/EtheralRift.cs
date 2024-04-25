using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.isartdigital.f2p.gameplay.manager;
using Com.IsartDigital.F2P.Biomes;
using UnityEngine;

namespace Com.IsartDigital.F2P.Gameplay.Events
{
    public class EtheralRift : GameRandomEvent
    {
        private List<BiomeType> _BiomeTypeToReplace = new List<BiomeType>();

        protected override void Start()
        {
            base.Start();
            _Priority = 2;
        }
        protected override IEnumerator PlayRandomEventEffect()
        {
            foreach (Biome lBiome in GridManager.GetInstance().Biomes)
            {
                if(_GridManager.GetGridCoordinate(lBiome.transform.position).x == _GridPosition.x)
                {
                    _BiomeTypeToReplace = Enum.GetValues(typeof(BiomeType)).Cast<BiomeType>().ToList();

                    if (lBiome.transform.position == Player.GetInstance().transform.position)
                    {
                        _BiomeTypeToReplace.Remove(BiomeType.Canyon);
                    }
                    BiomeType lRandomType = _BiomeTypeToReplace[UnityEngine.Random.Range(0, _BiomeTypeToReplace.Count - 1)];
                    lBiome.Replace(CardPrefabDic.GetPrefab(lRandomType).transform);
                }
            }
            return null;
        }
    }
}
