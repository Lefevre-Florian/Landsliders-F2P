using System;

using UnityEngine;
using UnityEngine.Events;

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
        [SerializeField] private bool _IsRemoving = true;
        [SerializeField][Range(0, 10)] private int _NbTurn = 0;

        // Variables
        private int _Timer = 0;

        private GameManager _GameManager = null;
        private HandManager _HandManager = null;

        private void Start()
        {
            _HandManager = HandManager.GetInstance();
            _GameManager = GameManager.GetInstance();
        }

        public void StartContinous()
        {
            _Timer = _NbTurn;
            _GameManager.OnTurnPassed += UpdateTime;
            UpdateTime();
        }

        public void StopContinuous() => _GameManager.OnTurnPassed -= UpdateTime;

        public void SetNbCardAffected(int pNb) => _NbAffected = pNb;

        private void UpdateTime()
        {
            if (--_Timer <= 0)
                _Timer = 0;
            else
                UpdateDeck();
        }

        private void UpdateDeck()
        {
            if (_IsRemoving && !Player.GetInstance().isProtected)
                _HandManager.BurnCard(_NbAffected);  
            else if(!_IsRemoving)
                _HandManager.AddCardToDeck(_NbAffected);
        }

        private void OnDestroy()
        {
            _GameManager.OnTurnPassed -= UpdateTime;

            _HandManager = null;
            _GameManager = null;
        }
    }
}
