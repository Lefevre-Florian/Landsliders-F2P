using com.isartdigital.f2p.gameplay.manager;

using Com.IsartDigital.F2P.Gameplay.Manager;
using Com.IsartDigital.F2P.Sound;

using UnityEngine;

// Author (CR): Dorian Husson
namespace Com.IsartDigital.F2P.Gameplay.Events
{
    public class GameRandomEvent : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField][Min(0)] protected int _Priority;

        [Header("Sound")]
        [SerializeField] private SoundEmitter _SpawnSFXEmitter = null;

        protected Vector2 _GridPosition;

        protected GameManager _GameManager = GameManager.GetInstance();
        protected GridManager _GridManager = GridManager.GetInstance();
        protected HandManager _HandManager = HandManager.GetInstance();
        protected Player _Player = Player.GetInstance();

        protected virtual void Start()
        {
            _GameManager.OnEffectPlayed += OnRandomEventTriggered;
            _GridPosition = GridManager.GetInstance().GetGridCoordinate(transform.position);

            if (_SpawnSFXEmitter != null)
                _SpawnSFXEmitter.PlaySFXOnShot();
        }

        protected void OnRandomEventTriggered(int pPriority)
        {
            if (pPriority == _Priority)
            {
                PlayRandomEventEffect();
            }
        }

        protected virtual void PlayRandomEventEffect()
        {

        }

        protected void OnDestroy()
        {
            _GameManager.OnEffectPlayed -= OnRandomEventTriggered;

            if (GameRandomEventsManager.GetInstance().GameEventCount > 0)
            {
                GameRandomEventsManager.GetInstance().GameEventCount -= 1;
            }
        }
    }
}
