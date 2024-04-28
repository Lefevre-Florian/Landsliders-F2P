using System;

namespace Com.IsartDigital.F2P
{
    [Serializable]
    public class PlayerSave
    {
        // Gameplay related
        public float softcurrency = 0f;
        public float hardcurrency = 0f;

        public float exp = 0f;

        public int[] cards = null;

        // Settings
        public bool soundStatus = true;
        public float musicVolume = 0f;
        public float sfxVolume = 0f;
    }
}
