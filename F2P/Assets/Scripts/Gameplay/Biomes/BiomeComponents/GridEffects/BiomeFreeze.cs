using com.isartdigital.f2p.gameplay.manager;

using System;
using System.Collections.Generic;
using System.Linq;

using Unity.VisualScripting;
using UnityEngine;

namespace Com.IsartDigital.F2P.Biomes
{
    [RequireComponent(typeof(Biome))]
    public class BiomeFreeze : MonoBehaviour
    {
        [Header("Design")]
        [SerializeField] private BiomeType[] _StoppingBiome = new BiomeType[] { BiomeType.Canyon };

        // Variables
        private GridManager _GridManager = null;

        private Vector2 _GridPosition = new Vector2();

        private Biome _Biome = null;

        private void Awake()
        {
            _Biome = GetComponent<Biome>();
            _Biome.OnReady += Enable;
        }

        private void Start() => _GridManager = GridManager.GetInstance();

        public void Spread()
        {
            List<Biome> lBiomes = _GridManager.Biomes;
            if (lBiomes.Count == 0)
                return;

            lBiomes.RemoveAll(x => !x.CanBeReplaced);
            lBiomes.Remove(_GridManager.GetCardByGridCoordinate(Player.GetInstance().GridPosition));
            lBiomes.RemoveAll(x => x.Type == _Biome.Type);

            int lIdx = UnityEngine.Random.Range(0, lBiomes.Count);
            lBiomes[lIdx].AddComponent<BiomeFreeze>();
        }

        private void Enable()
        {
            GameManager.PlayerMoved.AddListener(Contact);
            _GridPosition = _Biome.GridPosition;
        }

        private void Contact()
        {
            Vector2 lPlayerPosition = Player.GetInstance().GridPosition;
            if (_GridPosition == lPlayerPosition)
                Slide();
        }

        private void Slide()
        {
            Player lPlayer = Player.GetInstance();
            Vector2 lDirection = (lPlayer.GridPosition - lPlayer.PreviousGridPosition).normalized;
            Vector2 lNextPosition = new Vector2(lPlayer.GridPosition.x + lDirection.x, lPlayer.GridPosition.y + lDirection.y);

            // Check if the next position is valid
            if (lNextPosition.x > _GridManager._GridSize.x
               || lNextPosition.x < 0
               || lNextPosition.y < 0
               || lNextPosition.y > _GridManager._GridSize.y)
                return;

            Biome lNextBiome = _GridManager.GetCardByGridCoordinate(lNextPosition);
            if(lNextBiome != null 
                && !_StoppingBiome.Contains(lNextBiome.Type))
                lPlayer.SetModeSlide(lNextPosition);
        }

        private void OnDestroy()
        {
            if(_Biome != null)
            {
                _Biome.OnReady -= Enable;
                _Biome = null;
            }

            GameManager.PlayerMoved.RemoveListener(Contact);
            _GridManager = null;
        }
    }
}
