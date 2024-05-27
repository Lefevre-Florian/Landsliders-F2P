using com.isartdigital.f2p.gameplay.quest;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes.Effects
{
    public class DeckEffect : MonoBehaviour
    {
        public enum AlterationType
        {
            Negative = -1,
            Positive = 1
        }

        // Variables
        private HandManager _Deck = null;
        private Player _Player = null;

        private GameManager _Clock = null;

        private float _Timer = 0;
        private AlterationType _Type;

        private int _ImpactStats = 1;

        private bool _IsSkipping = true;

        private void Start()
        {
            _Player = Player.GetInstance();
            _Deck = HandManager.GetInstance();
        }

        public void SetEffect(int pDuration, int pStats, AlterationType pType)
        {
            _Timer = pDuration;
            _Type = pType;

            _ImpactStats = pStats;

            _Clock = GameManager.GetInstance();
            _Clock.OnTurnPassed += UpdateTime;
        }

        [HideInInspector]
        public void IncrementTimer() => _Timer += 1;

        private void UpdateTime()
        {
            if (_IsSkipping)
            {
                _IsSkipping = false;
                return;
            }

            PlayEffect();
            if (--_Timer <= 0)
            {
                _Timer = 0;
                Destroy(this);
            }
        }

        private void PlayEffect()
        {
            if (_Type == AlterationType.Negative && !_Player.isProtected)
                _Deck.BurnCard(_ImpactStats);
            else if (_Type == AlterationType.Positive)
                _Deck.AddCardToDeck(_ImpactStats);
        }

        private void OnDestroy()
        {
            if (_Clock != null)
                _Clock.OnTurnPassed -= UpdateTime;
            _Clock = null;

            _Player = null;
            _Deck = null;
        }
    }
}
