using Com.IsartDigital.F2P.Sound;

using UnityEngine;

// Author (CR): Dorian Husson
namespace Com.IsartDigital.F2P.Gameplay.Events
{
    public class Wisp : GameRandomEvent
    {
        [Header("Settings")]
        [SerializeField] private int _NbCardsAdded;
        [SerializeField] private int _TurnBeforeDestroy = 2;

        [Header("Sound")]
        [SerializeField] private SoundEmitter _EffectSFXEmitter = null;

        [Header("Juiciness")]
        [SerializeField] private GameObject _Particles;
        
        // Variables
        private int _TurnCount;

        protected override void PlayRandomEventEffect()
        {
            if (_GridManager.GetGridCoordinate(transform.position) == _GridManager.GetGridCoordinate(_Player.transform.position))
            {
                if (_EffectSFXEmitter != null)
                    _EffectSFXEmitter.PlaySFXOnShot();

                _HandManager.AddCardToDeck(_NbCardsAdded);

                Instantiate(_Particles, transform.position, Quaternion.identity);

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
