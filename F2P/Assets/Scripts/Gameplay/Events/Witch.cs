using System;
using UnityEngine;

// Author: Dorian Husson
namespace Com.IsartDigital.F2P.Gameplay.Events
{
    public class Witch : GameRandomEvent
    {
        public event Action OnWitchPosition;
        protected override void PlayRandomEventEffect()
        {
            if (_GridManager.GetGridCoordinate(transform.position) == _GridManager.GetGridCoordinate(_Player.transform.position))
            {
                OnWitchPosition?.Invoke();

                Destroy(transform.gameObject);
            }
        }
    }
}
