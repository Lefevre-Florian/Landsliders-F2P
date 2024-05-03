using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    public class BiomeSanctuary : MonoBehaviour
    {
        private Player _Player = null;

        private void Start() => _Player = Player.GetInstance();

        /// <summary>
        /// Player will be protected but won't move
        /// </summary>
        public void EnableSanctuary()
        {
            _Player.isProtected = true;
        }

        public void DisableSanctuary()
        {
            _Player.isProtected = false;
        }

        private void OnDestroy() => _Player = null;
    }
}
