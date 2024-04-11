using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.IsartDigital.F2P.Biomes
{
    [RequireComponent(typeof(BiomeSurrondingAnalysis))]
    public class FieldBiome : MonoBehaviour
    {
        [Header("Design - Settings")]
        [SerializeField] private int _Bonus = 0;
        [SerializeField] private int _GiftCard = 1;

        [SerializeField] private Action Method;

        private void OnPlayerCollision()
        {
            GetComponent<BiomeSurrondingAnalysis>().GetSurrounding();
        }
    }
}
