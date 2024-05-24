// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Sound
{
    public class MusicEmiiterFocused : MusicEmitter
    {
  
        protected override void Start()
        {
            base.Start();
            TEMPCard.OnFocus += PerformFocus;
        }
        
        private void PerformFocus(bool pState)
        {
            if (pState)
                SetImmediateFade(1);
            else
                SetImmediateFade(0);
        }

        protected override void OnDestroy()
        {
            TEMPCard.OnFocus -= PerformFocus;
            base.OnDestroy();
        }
    }
}
