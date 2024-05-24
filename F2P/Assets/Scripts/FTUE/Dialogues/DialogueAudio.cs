using FMODUnity;

using System;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FTUE.Dialogues
{
    [Serializable]
    public class DialogueAudio
    {
        [SerializeField] private EventReference _Audio;
        [SerializeField] private float _Duration;
        [SerializeField] private float _FMODKey = 0f;

        public EventReference Audio { get { return _Audio; } }

        public float Duration { get { return _Duration; } }

        public float FMODKey { get { return _FMODKey; } }

    }
}
