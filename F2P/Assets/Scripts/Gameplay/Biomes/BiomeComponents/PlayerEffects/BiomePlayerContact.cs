using UnityEngine;
using UnityEngine.Events;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    [RequireComponent(typeof(Biome))]
    public class BiomePlayerContact : MonoBehaviour
    {
        private const int CARDINAL_AOE = 4;

        [Header("Architecture")]
        [SerializeField] private UnityEvent _OnPlayerCollision = null;

        // Variables
        private Biome _Biome = null;

        private bool _CollisionEnabled = true;

        private void Start() => _Biome = GetComponent<Biome>();

        public void ComputeCollision()
        {
            if (!_CollisionEnabled)
                return;

            // Collision
            if (_Biome.GridPosition == Player.GetInstance().GridPosition)
                _OnPlayerCollision?.Invoke();
        }

        public void ComputeAOECollision()
        {
            if (!_CollisionEnabled)
                return;

            // Collision computation (AOE)
            float lAngle = ((Mathf.PI * 2f) / CARDINAL_AOE) * Mathf.Rad2Deg;
            Player lPlayer = Player.GetInstance();

            Vector2 lGridPosition;
            for (int i = 0; i < CARDINAL_AOE; i++)
            {
                lGridPosition = _Biome.GridPosition + (Vector2)(Quaternion.AngleAxis(lAngle * i, Vector3.forward) * Vector3.up);
                if(lGridPosition == lPlayer.GridPosition)
                {
                    _OnPlayerCollision?.Invoke();
                    return;
                }
            }

            ComputeCollision();
        }

        public void EnableCollision() => _CollisionEnabled = true;

        public void DisableCollision() => _CollisionEnabled = false;

        private void OnDestroy() => _Biome = null;
    }
}
