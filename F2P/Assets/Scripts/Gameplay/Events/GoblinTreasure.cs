using System;

using UnityEngine;

using Com.IsartDigital.F2P.FileSystem;
using Com.IsartDigital.F2P.Sound;
using Com.IsartDigital.F2P.UI.UIHUD;

// Author (CR): Dorian Husson
namespace Com.IsartDigital.F2P.Gameplay.Events
{
    public class GoblinTreasure : GameRandomEvent
    {
        [Header("Settings")]
        [SerializeField] private int _NbCardsAdded = 3;
        [SerializeField] private int _NbCardsBurnt = 2;
        [SerializeField] [Range(0f, 1f)] private float _TreasureChance = 0.5f;
        [SerializeField] private int _SoftCurrencyReward = 10;

        [Header("Sound")]
        [SerializeField] private SoundEmitter _PositiveOutcomeSFXEmitter = null;
        [SerializeField] private SoundEmitter _NegativeOutcomeSFXEmitter = null;
        
        [Header("Juiciness")]        
        [SerializeField] private GameObject _ParticlesWin;
        [SerializeField] private GameObject _ParticlesLoose;
        
        // Variables
        private float _RandomValue;

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

                    if (_PositiveOutcomeSFXEmitter != null)
                        _PositiveOutcomeSFXEmitter.PlaySFXOnShot();

                    Hud.GetInstance()._GoldBonus += 60;

                    Instantiate(_ParticlesWin, transform.position, Quaternion.identity);
                }
                else
                {
                    _HandManager.BurnCard(_NbCardsBurnt);

                    if (_NegativeOutcomeSFXEmitter != null)
                        _NegativeOutcomeSFXEmitter.PlaySFXOnShot();

                    Instantiate(_ParticlesLoose, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }
    }
}
