using Com.IsartDigital.F2P.Biomes;

using System;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.FTUE
{
    [CreateAssetMenu(fileName = "new deck", menuName = "FTUE/Deck", order = 0)]
    public class DeckSO : ScriptableObject
    {
        [SerializeField][Range(1,3)] private int _FTUEPhase = 1;
        [SerializeField] private Deck _Deck = null;
        [SerializeField][Range(1,4)] private int _StartNBCards = 1;

        public int FTUEPhase { get { return _FTUEPhase; } }
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
