using UnityEngine;
using UnityEngine.Events;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    [RequireComponent(typeof(Biome))]
    public class BiomePlayerContact : MonoBehaviour
    {
        [Header("Architecture")]
        [SerializeField] private UnityEvent _OnPlayerCollision = null;

        // Variables
        private Biome _Biome = null;

        private void Start() => _Biome = GetComponent<Biome>();

        public void ComputeCollision()
        {
            // Collision
            if (_Biome.GridPosition == Player.GetInstance().GridPosition)
                _OnPlayerCollision?.Invoke();
        }

        private void OnDestroy() => _Biome = null;
    }
}
