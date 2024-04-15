using com.isartdigital.f2p.gameplay.manager;

using System;
using System.Linq;

using UnityEngine;

namespace Com.IsartDigital.F2P.Biomes
{
    public class BiomeFreeze : MonoBehaviour
    {
        [Header("Design")]
        [SerializeField] private BiomeType[] _StoppingBiome = new BiomeType[] { BiomeType.Canyon };

        // Variables
        private Player _Player = null;
        private GridManager _GridManager = null;

        private Vector2 _GridPosition = new Vector2();

        private void Start()
        {
            _GridManager = GridManager.GetInstance();

            // Remove later
            Enable();
        }

        private void Enable()
        {
            _Player = Player.GetInstance();
            GameManager.PlayerMoved.AddListener(Contact);

            _GridPosition = _GridManager.GetGridCoordinate(transform.position);
        }

        public void Spread()
        {
            Biome[,] lBiomes = _GridManager.Biomes;
            if (lBiomes.Length == 0)
                return;

            ///TODO : Spread on random excluding me and other frozen biomes
        }

        private void Slide()
        {
            Vector2 lDirection = (_Player.PreviousGridPosition - _Player.GridPosition).normalized;
            Vector2 lNextPosition = new Vector2(_Player.GridPosition.x + lDirection.x, _Player.GridPosition.y + lDirection.y);

            // Check if the next position is valid
            if (lNextPosition.x > _GridManager._GridSize.x
               || lNextPosition.x < 0
               || lNextPosition.y < 0
               || lNextPosition.y > _GridManager._GridSize.y)
                return;

            Biome lNextBiome = _GridManager.GetCardByGridCoordinate(lNextPosition);
            if(lNextBiome != null 
                && !_StoppingBiome.Contains(lNextBiome.Type)
                && lNextBiome.GetComponent<BiomeFreeze>() != null)
            {
                Debug.Log(lNextPosition);
                _Player.SetNextPosition(lNextPosition);
                _Player.SetModeMove();
            }
        }

        private void Defrost()
        {
            GameManager.PlayerMoved.RemoveListener(Contact);
            Destroy(this);
        }

        private void Contact()
        {
            if (_GridPosition == _Player.GridPosition)
                Slide();
        }

        private void OnDestroy()
        {
            _GridManager = null;
            if(_Player != null)
            {
                _Player = null;
            }
        }
    }
}
