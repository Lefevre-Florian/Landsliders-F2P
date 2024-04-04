using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI
{
    public class Screen : MonoBehaviour
    {
        public virtual void Open() => gameObject.SetActive(true);

        public virtual void Open(Screen pScreen) => pScreen.Open();

        public virtual void Close() => gameObject.SetActive(false);

        public virtual void Close(Screen pScreen) => pScreen.Close();

        public void Navigate(Screen pTargetScreen)
        {
            Close();
            pTargetScreen.Open();
        }
    }
}
