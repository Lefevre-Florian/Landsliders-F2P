using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI
{
    public abstract class Screen : MonoBehaviour
    {
        public virtual void Open() => gameObject.SetActive(true);

        public virtual void Close() => gameObject.SetActive(false);

        public void Navigate(Screen pTargetScreen)
        {
            Close();
            pTargetScreen.Open();
        }
    }
}
