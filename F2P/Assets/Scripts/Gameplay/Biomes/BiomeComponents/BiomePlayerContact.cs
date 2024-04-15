using com.isartdigital.f2p.gameplay.manager;

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
        private Player _Player = null;
        private Biome _Biome = null;

        private void Start()
        {
            _Biome = GetComponent<Biome>();
            _Biome.onTriggered.AddListener(ComputeCollision);

            _Player = Player.GetInstance();
        }

        private void ComputeCollision()
        {
            // Collision
            if (_Biome.GridPosition == _Player.GridPosition)
                _OnPlayerCollision?.Invoke();
        }

        private void OnDestroy()
        {
            _Biome.onTriggered.RemoveListener(ComputeCollision);
            _Biome = null;

            _Player = null;
        }
    }
}
