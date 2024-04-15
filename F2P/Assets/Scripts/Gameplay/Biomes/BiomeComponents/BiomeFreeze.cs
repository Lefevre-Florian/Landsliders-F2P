using com.isartdigital.f2p.gameplay.manager;

using System;

using UnityEngine;

namespace Com.IsartDigital.F2P.Biomes
{
    [RequireComponent(typeof(BiomeSurrondingAnalysis))]
    public class BiomeFreeze : MonoBehaviour
    {

        [Header("Design")]
        [SerializeField] private MonoBehaviour[] _FreezeComponents = new MonoBehaviour[0];

        // Variables
        private Player _Player = null;
        private GridManager _GridManager = null;

        private void Start()
        {
            _Player = Player.GetInstance();
            _GridManager = GridManager.GetInstance();
        }

        public void Slide()
        {
            Vector2 lDirection = (_Player.PreviousGridPosition - _Player.GridPosition).normalized;
            Vector2 lNextPosition = new Vector2(_Player.GridPosition.x + lDirection.x, _Player.GridPosition.y + lDirection.y);

            // Check if the next position is valid
            if (lNextPosition.x > _GridManager._GridSize.x
               || lNextPosition.x < 0
               || lNextPosition.y < 0
               || lNextPosition.y > _GridManager._GridSize.y)
                return;

            BiomeFreeze lNextBiome;
            if(_GridManager.GetCardByGridCoordinate(lNextPosition).TryGetComponent<BiomeFreeze>(out lNextBiome))
            {
                _Player.SetNextPosition(lNextPosition);
                _Player.SetModeMove();
            }
        }

        private void OnDestroy()
        {
            _GridManager = null;
            _Player = null;
        }

    }
}
