using System;

namespace Com.IsartDigital.F2P
{
    [Serializable]
    public class PlayerSave
    {
        public bool musicStatus = true;
        public bool soundStatus = true;

        public float softcurrency = 0f;
        public float hardcurrency = 0f;

        public float exp = 0f;
    }
}
