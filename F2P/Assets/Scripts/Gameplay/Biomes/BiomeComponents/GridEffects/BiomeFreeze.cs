using com.isartdigital.f2p.gameplay.manager;
using Com.IsartDigital.F2P.Sound;

using System;
using System.Collections.Generic;
using System.Linq;

using Unity.VisualScripting;
using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    [RequireComponent(typeof(Biome))]
    public class BiomeFreeze : MonoBehaviour, IBiomeSupplier
    {
        [Header("Design")]
        [SerializeField] private BiomeType[] _StoppingBiome = new BiomeType[] { BiomeType.Canyon };

        [Header("Feedback & Juiciness")]
        [SerializeField] private Transform _VFXFrozen = null;
        [SerializeField] private SoundEmitter _SFXFrozenEmitter = null;

        // Variables
        private GridManager _GridManager = null;

        private Vector2 _GridPosition = new Vector2();

        private Biome _Biome = null;

        private void Start()
        {
            _Biome = GetComponent<Biome>();
            if (!_Biome.IsReady)
                _Biome.OnReady += Enable;
            else
                Enable();

            _GridManager = GridManager.GetInstance();
        }

        public void Spread() => PerformSpreading(ComputeTarget());

        public void Spread(MonoBehaviour pSupplier)
        {
            if (pSupplier is IBiomeSupplier)
                PerformSpreading((pSupplier as IBiomeSupplier).SupplyBiomes()[0]);
            else
                Debug.LogError($"Must be of type : {typeof(IBiomeSupplier)}");
        }

        [HideInInspector]
        public Vector2[] SupplyBiomes() => new Vector2[] { ComputeTarget() };

        private void PerformSpreading(Vector2 pTarget)
        {
            if (pTarget == -Vector2.one)
                return;

            if (_SFXFrozenEmitter != null)
                _SFXFrozenEmitter.PlaySFXOnShot();

            Biome lBiome = _GridManager.GetCardByGridCoordinate(pTarget);

            lBiome.AddComponent<BiomeFreeze>();
            lBiome.GetComponent<Biome>().locked = true;

            Instantiate(_VFXFrozen, lBiome.transform);
        }

        private Vector2 ComputeTarget()
        {
            List<Biome> lBiomes = _GridManager.Biomes;
            if (lBiomes.Count == 0)
                return -Vector2.one;

            // Add condition to the spread
            lBiomes.RemoveAll(x => !x.CanBeReplaced);
            lBiomes.Remove(_GridManager.GetCardByGridCoordinate(Player.GetInstance().GridPosition));
            lBiomes.RemoveAll(x => x.Type == _Biome.Type);
            lBiomes.RemoveAll(x => x.GetComponent<BiomeFreeze>() != null);
            lBiomes.RemoveAll(x => _StoppingBiome.Contains(x.Type));
            lBiomes.RemoveAll(x => x == null);

            // Recheck biomes states after every conditions were applied
            if (lBiomes.Count == 0)
                return -Vector2.one;

            int lIdx = UnityEngine.Random.Range(0, lBiomes.Count);
            return lBiomes[lIdx].GridPosition;
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
            Vector2 lNextPosition = new Vector2(Mathf.RoundToInt(lPlayer.GridPosition.x + lDirection.x), 
                                                Mathf.RoundToInt(lPlayer.GridPosition.y + lDirection.y));

            // Check if the next position is valid
            if (lNextPosition.x > _GridManager._NumCard.x - 1
               || lNextPosition.x < 0
               || lNextPosition.y < 0
               || lNextPosition.y > _GridManager._NumCard.y - 1)
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
