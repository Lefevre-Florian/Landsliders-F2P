using System;
using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI
{
    public class Screen : MonoBehaviour
    {
        // Events
        public event Action OnScreenOpened;
        public event Action OnScreenClosed;

        public virtual void Open()
        {
            OnScreenOpened?.Invoke();
            gameObject.SetActive(true);
        }

        public virtual void Open(Screen pScreen) => pScreen.Open();

        public virtual void Close()
        {
            OnScreenClosed?.Invoke();
            gameObject.SetActive(false);
        }

        public virtual void Close(Screen pScreen) => pScreen.Close();

        public void Navigate(Screen pTargetScreen)
        {
            Close();
            pTargetScreen.Open();
        }
    }
}
