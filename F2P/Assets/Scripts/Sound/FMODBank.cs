using FMODUnity;

using System;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Sound
{
    [Serializable]
    public class FMODBank
    {
        // Export
        [BankRef] public string path = default;
        [SerializeField] private SoundType _Type = SoundType.GLOBAL;

        // Variables
        private int _BankID = 0;

        [HideInInspector] public bool isLoaded = false;

        // Get / Set
        public int BankID { get { return _BankID; } }

        public SoundType Category { get { return _Type; } }

        public void SetBankID(int pID) => _BankID = pID;
    }
}
