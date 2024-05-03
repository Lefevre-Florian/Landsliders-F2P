using System;
using System.Collections;
using com.isartdigital.f2p.gameplay.manager;
using UnityEngine;

// Author: Dorian Husson
namespace Com.IsartDigital.F2P.Gameplay.Events
{
    public class DragonLair : GameRandomEvent
    {
        [SerializeField] private GameObject _Dragon;
        [SerializeField] private int _NbCardsAdded;

        protected override void Start()
        {
            base.Start();
            Instantiate(_Dragon, transform.position, Quaternion.identity);
        }

        protected override void PlayRandomEventEffect()
        {
            if (_GridManager.GetGridCoordinate(transform.position) == _GridManager.GetGridCoordinate(_Player.transform.position))
            {
                _HandManager.AddCardToDeck(_NbCardsAdded);

                Destroy(transform.gameObject);
            }
        }
    }
}
