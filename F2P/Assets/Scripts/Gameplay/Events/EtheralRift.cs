using System;
using System.Collections.Generic;
using System.Linq;

using Com.IsartDigital.F2P.Biomes;
using Com.IsartDigital.F2P.Sound;

using UnityEngine;

// Author (CR): Dorian Husson
namespace Com.IsartDigital.F2P.Gameplay.Events
{
    public class EtheralRift : GameRandomEvent
    {
        [Header("Sound")]
        [SerializeField] private SoundEmitter _EffectSFXEmitter = null;
        
        [Header("Juiciness")]
        [SerializeField] private GameObject _Particles;

        // Variables
        private List<BiomeType> _BiomeTypeToReplace = new List<BiomeType>();

        protected override void PlayRandomEventEffect()
        {
            if (_EffectSFXEmitter != null)
                _EffectSFXEmitter.PlaySFXOnShot();

            foreach (Biome lBiome in _GridManager.Biomes)
            {
                if(_GridManager.GetGridCoordinate(lBiome.transform.position).x == _GridPosition.x)
                {
                    _BiomeTypeToReplace = Enum.GetValues(typeof(BiomeType)).Cast<BiomeType>().ToList();

                    if (lBiome.transform.position == _Player.transform.position)
                    {
                        _BiomeTypeToReplace.Remove(BiomeType.Canyon);
                    }
                    BiomeType lRandomType = _BiomeTypeToReplace[UnityEngine.Random.Range(0, _BiomeTypeToReplace.Count - 1)];
                    lBiome.Replace(CardPrefabDic.GetPrefab(lRandomType).transform);

                    Instantiate(_Particles, lBiome.transform.position, Quaternion.identity);
                }
            }
        }
    }
}
