using UnityEngine;

namespace Com.IsartDigital.F2P.Biomes
{
    public class BiomeProtection : MonoBehaviour
    {
        // Variables
        private Player _Player = null;

        private void Start() => _Player = Player.GetInstance();

        public void EnablePlayerProtection() => _Player.isProtected = true;

        public void DisablePlayerProtection() => _Player.isProtected = false;

        private void OnDestroy() => _Player = null;

    }
}
