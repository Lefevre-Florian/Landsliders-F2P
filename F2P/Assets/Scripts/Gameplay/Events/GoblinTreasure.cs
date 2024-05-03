using System;
using UnityEngine;

// Author (CR): Dorian Husson
namespace Com.IsartDigital.F2P.Gameplay.Events
{
    public class GoblinTreasure : GameRandomEvent
    {
        [SerializeField] private int _NbCardsAdded = 3;
        [SerializeField] private int _NbCardsBurnt = 2;
        [SerializeField] [Range(0f, 1f)] private float _TreasureChance = 0.5f;
        private float _RandomValue;

        protected override void PlayRandomEventEffect()
        {
            if (_GridManager.GetGridCoordinate(transform.position) == _GridManager.GetGridCoordinate(_Player.transform.position))
            {
                _RandomValue = UnityEngine.Random.value;

                if (_RandomValue <= _TreasureChance)
                {
                    _HandManager.AddCardToDeck(_NbCardsAdded);
                }
                else
                {
                    _HandManager.BurnCard(_NbCardsBurnt);
                }

                Destroy(gameObject);
            }
        }
    }
}
