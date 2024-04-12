using System;
using UnityEngine;

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
        [SerializeField] private AffectingType _Type = AffectingType.Immediate;
        [SerializeField][Range(0, 10)] private int _NbTurn = 0;

        // Variables
        private int _Timer = 0;

        private GameManager _GameManager = null;

        private void Start() => _GameManager = GameManager.GetInstance();

        public void StartContinous()
        {
            _Timer = _NbTurn;
            if (_Type == AffectingType.Continuious)
                _GameManager.OnTurnPassed += UpdateTime;
            UpdateTime();
        }

        public void StopContinuous() => _GameManager.OnTurnPassed -= UpdateTime;

        private void UpdateTime()
        {
            if (--_Timer <= _NbTurn)
                _Timer = 0;
            else
                UpdateDeck();
        }

        private void UpdateDeck() => _GameManager.cardStocked += (_IsRemoving) ? -_NbAffected : _NbAffected;

        private void OnDestroy()
        {
            if (_Type == AffectingType.Continuious)
                StopContinuous();

            _GameManager = null;
        }
    }
}
