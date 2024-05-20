using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    public class BiomeSanctuary : MonoBehaviour
    {
        [Header("Juiciness")]
        [SerializeField] private bool _IsLooping = false;
        [SerializeField] private GameObject _Particles;

        // Variables
        private Player _Player = null;

        private BiomeParticles _ParticlesSystem = null;

        private void Start()
        {
            _Player = Player.GetInstance();
            _ParticlesSystem = GetComponent<BiomeParticles>();
        }

        /// <summary>
        /// Player will be protected but won't move
        /// </summary>
        public void EnableSanctuary()
        {
            _Player.isProtected = true;

            if(_ParticlesSystem != null 
                && _Particles != null)
            {
                if (_IsLooping)
                    _ParticlesSystem.PlayLoopParticles(this, _Particles);
                else
                    _ParticlesSystem.PlayOneshotParticles(_Particles);
            }
        }

        public void DisableSanctuary()
        {
            _Player.isProtected = false;

            if (_ParticlesSystem != null 
                && _IsLooping 
                && _Particles != null)
                _ParticlesSystem.StopLoopParticles(this);
        }

        private void OnDestroy() => _Player = null;
    }
}
