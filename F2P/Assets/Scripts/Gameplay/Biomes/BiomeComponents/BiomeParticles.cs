using System;
using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    public class BiomeParticles : MonoBehaviour
    {
        // Variables
        private ParticleSystem _Loop = null;

        public void PlayOneshotParticles(GameObject pPrefab) => CreateParticles(pPrefab);

        public void PlayLoopParticles(GameObject pPrefab)
        {
            _Loop = CreateParticles(pPrefab, true);
        }

        public void StopLoopParticles()
        {
            if(_Loop != null)
            {
                ParticleSystem.MainModule lSystem = _Loop.main;
                lSystem.loop = false;
            }
        }

        private ParticleSystem CreateParticles(GameObject pPrefab, bool pIsLooping = false)
        {
            ParticleSystem lParticles = Instantiate(pPrefab, transform).GetComponent<ParticleSystem>();
            lParticles.Stop();

            ParticleSystem.MainModule lSystem = lParticles.main;
            lSystem.duration = GameManager.GetInstance().EffectDuration;
            
            lSystem.loop = pIsLooping;
            lSystem.stopAction = ParticleSystemStopAction.Destroy;

            lParticles.Play();

            return lParticles;
        }
    }
}
