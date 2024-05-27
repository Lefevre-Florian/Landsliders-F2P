using Com.IsartDigital.F2P.Sound;
using UnityEngine;

// Author (CR): Dorian Husson
namespace Com.IsartDigital.F2P.Gameplay.Events
{
    public class DragonLair : GameRandomEvent
    {
        [SerializeField] private GameObject _Dragon;
        [SerializeField] private int _NbCardsAdded;

        [Header("Sound")]
        [SerializeField] private SoundEmitter _DragonCollectedSFXEmitter = null;

        protected override void Start()
        {
            base.Start();
            Instantiate(_Dragon, transform.position, Quaternion.identity);
        }

        protected override void PlayRandomEventEffect()
        {
            if (_GridManager.GetGridCoordinate(transform.position) == _GridManager.GetGridCoordinate(_Player.transform.position))
            {
                _DragonCollectedSFXEmitter.PlaySFXOnShot();
                _HandManager.AddCardToDeck(_NbCardsAdded);
                Destroy(gameObject);
            }
        }
    }
}
