using UnityEngine;

namespace Com.IsartDigital.F2P.Biomes
{
    public class Biome : MonoBehaviour
    {
        // Variables
        private int _Priority = 0;

        // Membres
        protected GameManager m_GameManager = null;

        protected virtual void Start()
        {
            m_GameManager = GameManager.GetInstance();
        }

        protected virtual void PlayEffect(int pGameCurrentPriority)
        {
            if (pGameCurrentPriority != _Priority)
                return;
        }

        protected virtual void OnDestroy()
        {
            m_GameManager = null;
        }

    }
}
