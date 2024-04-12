using UnityEngine;
using UnityEngine.Events;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    public class Biome : MonoBehaviour
    {
        [Header("Design")]
        [SerializeField][Min(0)] private int _Priority = 1;
        [SerializeField][Range(0f, 100f)] private float _EventProcChance = 50f;

        [SerializeField] private UnityEvent _OnTriggered = null;

        [Space(2)]
        [SerializeField] private BiomeType _Type = BiomeType.grassland;

        // Variables
        private GameManager _GameManager = null;

        private void Start()
        {
            _GameManager = GameManager.GetInstance();
            if(_Priority != 0)
                _GameManager.OnEffectPlayed += TriggerPriority;
        }

        private void TriggerPriority(int pGamePriority) 
        {
            if(pGamePriority != _Priority)
                _OnTriggered.Invoke(); 
        }

        private void OnDestroy()
        {
            if(_Priority != 0)
                _GameManager.OnEffectPlayed -= TriggerPriority;

            _OnTriggered = null;
            _GameManager = null;
        }
    }
}
