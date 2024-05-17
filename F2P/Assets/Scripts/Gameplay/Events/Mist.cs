using System;
using System.Collections.Generic;
using Com.IsartDigital.F2P.Biomes;
using UnityEngine;

// Author (CR): Dorian Husson
namespace Com.IsartDigital.F2P.Gameplay.Events
{
    public class Mist : GameRandomEvent
    {
        [SerializeField] private GameObject _Mist;
        private GameObject _InstantiatedMist;
        private List<GameObject> _Mistlist = new List<GameObject>();
  
        protected override void Start()
        {
            base.Start();

            foreach (Biome lBiome in _GridManager.Biomes)
            {
                if (_GridManager.GetGridCoordinate(lBiome.transform.position) != _GridManager.GetGridCoordinate(transform.position))
                {
                    _InstantiatedMist = Instantiate(_Mist);
                    _InstantiatedMist.transform.position = lBiome.transform.position;
                    _InstantiatedMist.transform.SetParent(transform);
                    _InstantiatedMist.transform.localPosition += new Vector3(0,0,-10);
                    _InstantiatedMist.SetActive(false);
                    _Mistlist.Add(_InstantiatedMist);
                }
            }
        }

        protected override void PlayRandomEventEffect()
        {
            if (_GridManager.GetGridCoordinate(transform.position) == _GridManager.GetGridCoordinate(_Player.transform.position))
            {
                foreach (GameObject lMist in _Mistlist)
                {
                    lMist.SetActive(true);
                }
            }
            else if(_InstantiatedMist.activeSelf)
            {
                foreach (GameObject lMist in _Mistlist)
                {
                    lMist.SetActive(false);
                }
            }
        }
    }
}
