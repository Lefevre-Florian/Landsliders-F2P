using com.isartdigital.f2p.gameplay.quest;
using System;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    public class BiomeAlterDeck : MonoBehaviour
    {
        private enum AffectingType
        {
            Continuious,
            Immediate
        }

        [Header("Design")]
        [SerializeField][Min(1)] private int _NbAffected = 1;

        [Space(2)]
        [SerializeField] private bool _IsRemoving = true;
        
        [Space(2)]
        [SerializeField][Range(0, 10)] private int _NbTurn = 0;

        // Variables
        private int _Timer = 0;

        private GameManager _GameManager = null;
        private HandManager _HandManager = null;

        private Player _Player = null;

        private void Start()
        {
            _HandManager = HandManager.GetInstance();
            _GameManager = GameManager.GetInstance();

            _Player = Player.GetInstance();
        }

        public void StartContinous()
        {
            _Timer = _NbTurn;
            _GameManager.OnTurnPassed += UpdateTime;
        }

        public void StopContinuous() => _GameManager.OnTurnPassed -= UpdateTime;

        public void ImmediateAlteration() => UpdateDeck();

        public void ImmmediateAlteration(MonoBehaviour pBonus)
        {
            if (pBonus is IBiomeEnumerator)
            {
                _NbAffected = (pBonus as IBiomeEnumerator).GetEnumertation();
                UpdateDeck();
            }
            else
                Debug.LogError("Must be an" + typeof(IBiomeEnumerator));
        }

        private void UpdateTime()
        {
            if (--_Timer <= 0)
                _Timer = 0;
            else
                UpdateDeck();
        }

        private void UpdateDeck()
        {
            if (_IsRemoving && !_Player.isProtected)
                _HandManager.BurnCard(_NbAffected);
            else if (!_IsRemoving)
            {
                _HandManager.AddCardToDeck(_NbAffected);
                if (TryGetComponent<FieldQuest>(out FieldQuest fq)) fq.CheckWin(_NbAffected);
            }
        }

        private void OnDestroy()
        {
            if (_GameManager != null)
                StopContinuous();

            _HandManager = null;
            _GameManager = null;

            _Player = null;
        }
    }
}