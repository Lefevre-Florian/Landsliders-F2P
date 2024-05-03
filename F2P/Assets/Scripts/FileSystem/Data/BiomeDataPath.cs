using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FileSystem
{
    [CreateAssetMenu(fileName = "Path", menuName = "Data")]
    public class BiomeDataPath : ScriptableObject
    {
        [SerializeField] private GameObject _Biomeref;

        public GameObject Biomeref { get { return _Biomeref; } }
    }
}
