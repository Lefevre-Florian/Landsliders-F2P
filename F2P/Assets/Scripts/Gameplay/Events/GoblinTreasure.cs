using System;
using UnityEngine;
using Com.IsartDigital.F2P.FileSystem;

// Author (CR): Dorian Husson
namespace Com.IsartDigital.F2P.Gameplay.Events
{
    public class GoblinTreasure : GameRandomEvent
    {
        [SerializeField] private int _NbCardsAdded = 3;
        [SerializeField] private int _NbCardsBurnt = 2;
        [SerializeField] [Range(0f, 1f)] private float _TreasureChance = 0.5f;
        [SerializeField] private int _SoftCurrencyReward = 60;
        private float _RandomValue;
        [SerializeField] private GameObject _ParticlesWin;
        [SerializeField] private GameObject _ParticlesLoose;

        protected override void PlayRandomEventEffect()
        {
            if (_GridManager.GetGridCoordinate(transform.position) == _GridManager.GetGridCoordinate(_Player.transform.position))
            {
                _RandomValue = UnityEngine.Random.value;

                if (_RandomValue <= _TreasureChance)
                {
                    _HandManager.AddCardToDeck(_NbCardsAdded);
                    Save.data.softcurrency += _SoftCurrencyReward;
                    DatabaseManager.GetInstance().WriteDataToSaveFile();

                    Instantiate(_ParticlesWin, transform.position, Quaternion.identity);
                }
                else
                {
                    _HandManager.BurnCard(_NbCardsBurnt);
                    Instantiate(_ParticlesLoose, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }
    }
}
