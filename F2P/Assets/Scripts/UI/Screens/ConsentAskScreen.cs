using System;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.UI.Screens
{
    public class ConsentAskScreen : Screen
    {

        // Event
        public event Action OnValidate;
        public event Action OnCanceled;
  
        public void Validate()
        {
            OnValidate?.Invoke();
            Close();
        }

        public void Cancel()
        {
            OnCanceled?.Invoke();
            Close();
        }
    }
}
