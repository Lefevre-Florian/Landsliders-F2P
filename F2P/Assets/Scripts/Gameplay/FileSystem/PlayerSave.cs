using System;

namespace Com.IsartDigital.F2P
{
    [Serializable]
    public class PlayerSave
    {
        public PlayerSave() { }

        public bool musicStatus = false;
        public bool soundStatus = false;

        public float softcurrency = 0f;
        public float hardcurrency = 0f;

        public float exp = 0f;
    }
}
