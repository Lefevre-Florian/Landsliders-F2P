using Com.IsartDigital.F2P.Biomes;

using System;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FTUE
{
    [CreateAssetMenu(fileName = "new phase", menuName = "FTUE/Phase", order = 0)]
    public class FTUEPhaseSO : ScriptableObject
    {
        [SerializeField][Range(1,3)] private int _FTUEPhase = 1;

        [Header("Deck & Hand")]
        [SerializeField] private Deck _Deck = null;
        [SerializeField][Range(1,4)] private int _StartNBCards = 1;

        [Space(5)]
        [Header("Player")]
        [SerializeField] private Vector2 _StartPosition = Vector2.one;

        public int FTUEPhase { get { return _FTUEPhase; } }

        /// FTUE DECK
        public Tuple<BiomeType, int>[] Deck { 
            get {
                int lLength = _Deck.cards.Length;
                Tuple<BiomeType, int>[] lCards = new Tuple<BiomeType, int>[lLength];
                for (int i = 0; i < lLength; i++)
                    lCards[i] = new Tuple<BiomeType, int>(_Deck.cards[i].type,
                                                          _Deck.cards[i].quantity);
                return lCards; 
            } 
        }

        public int StartNBCards { get { return _StartNBCards; } }

        /// FTUE PLAYER
        public Vector2 StartPosition { get { return _StartPosition; } }
    }

    [Serializable]
    public class Deck
    {
        public PairCardQuantity[] cards = new PairCardQuantity[0];
    }

    [Serializable]
    public class PairCardQuantity
    {
        public BiomeType type = default;
        [Min(1)] public int quantity = 1;
    }
}
