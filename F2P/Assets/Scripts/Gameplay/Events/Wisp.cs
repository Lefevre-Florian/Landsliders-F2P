using System;
using UnityEngine;

// Author (CR): Dorian Husson
namespace Com.IsartDigital.F2P.Gameplay.Events
{
    public class Wisp : GameRandomEvent
    {
        [SerializeField] private int _NbCardsAdded;
        [SerializeField] private int _TurnBeforeDestroy = 2;
        private int _TurnCount;
        protected override void PlayRandomEventEffect()
        {
            if (_GridManager.GetGridCoordinate(transform.position) == _GridManager.GetGridCoordinate(_Player.transform.position))
            {
                _HandManager.AddCardToDeck(_NbCardsAdded);

                Destroy(gameObject);
            }
            else if (_TurnCount == _TurnBeforeDestroy)
            {
                Destroy(gameObject);
            }

            _TurnCount += 1;
        }
    }
}
