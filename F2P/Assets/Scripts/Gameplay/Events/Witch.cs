using System;
using UnityEngine;
using UnityEngine.Events;

// Author (CR): Dorian Husson
namespace Com.IsartDigital.F2P.Gameplay.Events
{
    public class Witch : GameRandomEvent
    {
        public static UnityEvent OnWitchPosition = new UnityEvent();
        protected override void PlayRandomEventEffect()
        {
            if (_GridManager.GetGridCoordinate(transform.position) == _GridManager.GetGridCoordinate(_Player.transform.position))
            {
                OnWitchPosition?.Invoke();

                Destroy(gameObject);
            }
        }
    }
}
