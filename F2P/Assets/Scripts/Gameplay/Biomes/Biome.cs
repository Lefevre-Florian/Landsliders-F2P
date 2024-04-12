using UnityEngine;
using UnityEngine.Events;

namespace Com.IsartDigital.F2P.Biomes
{
    public class Biome : MonoBehaviour
    {
        [Header("Design")]
        [SerializeField][Min(0)] private int _Priority = 1;
        [SerializeField] private UnityEvent _OnTriggered = null;

        [Space(2)]
        [SerializeField] private BiomeType _Type = BiomeType.grassland;

        // Variables
        private GameManager _GameManager = null;

        private void Start()
        {
            _GameManager = GameManager.GetInstance();
            if(_Priority != 0)
            {

            }
        }

        private void TriggerPriority() => _OnTriggered.Invoke();

        private void OnDestroy()
        {
            if(_Priority != 0)
            {

            }
            _OnTriggered = null;
            _GameManager = null;
        }
    }
}
