using System;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public static class Save
    {
        public static PlayerSave data = null;
    }

    [Serializable]
    public class PlayerSave
    {
        // Gameplay related
        public float softcurrency = 0f;
        public float hardcurrency = 0f;

        public float exp = 0f;

        public int[] cards = null;

        public Fragment[] fragments = null;

        // Settings
        public bool soundStatus = true;
        public float musicVolume = 0f;
        public float sfxVolume = 0f;

        // Non-serializable
        [NonSerialized] public GameObject[] cardPrefabs;            // Only a session variable won't be include in save file
    }

    [Serializable]
    public class Fragment
    {
        public int id;
        public int fragment;

        [NonSerialized] public float rarity;

        public Fragment() { }

        public Fragment(int pId, int pFragmentCount)
        {
            id = pId;
            fragment = pFragmentCount;
        }

        public Fragment(int pId, int pFragmentCount, float pRarity)
        {
            id = pId;
            fragment = pFragmentCount;

            rarity = pRarity;
        }
    }
}