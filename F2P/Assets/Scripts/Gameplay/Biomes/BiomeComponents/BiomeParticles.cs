using System.Collections.Generic;
using System.Linq;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    public class BiomeParticles : MonoBehaviour
    {
        // Variables
        private Dictionary<MonoBehaviour, ParticleSystem> _Loops = new Dictionary<MonoBehaviour, ParticleSystem>();

        public void PlayOneshotParticles(GameObject pPrefab) => PlayOneshotParticles(pPrefab, transform.position);

        public void PlayOneshotParticles(GameObject pPrefab, Vector3 pPosition) => CreateParticles(pPrefab).transform.position = pPosition;

        public void PlayLoopParticles(MonoBehaviour pObj, GameObject pPrefab) => PlayLoopParticles(pObj, pPrefab, transform.position);

        public void PlayLoopParticles(MonoBehaviour pObj, GameObject pPrefab, Vector3 pPosition)
        {
            if (_Loops == null || _Loops.ContainsKey(pObj))
                return;

            ParticleSystem lParticles = CreateParticles(pPrefab, true);
            lParticles.transform.position = pPosition;

            _Loops.Add(pObj, lParticles);
        }

        public void StopLoopParticles(MonoBehaviour pObj)
        {
            if(_Loops != null && _Loops.ContainsKey(pObj))
            {
                ParticleSystem.MainModule lSystem = _Loops[pObj].main;
                lSystem.loop = false;

                _Loops.Remove(pObj);
            }
        }

        public void StopAllLoopParticles()
        {
            if(_Loops.Keys.Count > 0)
            {
                int lLength = _Loops.Count;
                for (int i = 0; i < lLength; i++)
                    StopLoopParticles(_Loops.Keys.ToList()[i]);
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
