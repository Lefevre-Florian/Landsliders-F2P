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
        [SerializeField] private TurnDeck[] _Decks = null;
        [SerializeField][Range(1,4)] private int _StartNBCards = 1;

        [Space(5)]
        [Header("Player")]
        [SerializeField] private Vector2 _StartPosition = Vector2.one;

        [Space(5)]
        [Header("Flow")]
        [SerializeField] private Phase[] _Phases = null;

        public int FTUEPhase { get { return _FTUEPhase; } }

        /// FTUE DECK
        public TurnDeck[] Decks { get { return _Decks; } }

        public int StartNBCards { get { return _StartNBCards; } }

        /// FTUE PLAYER
        public Vector2 StartPosition { get { return _StartPosition; } }

        /// FTUE Flow
        public Phase[] Phases { get { return _Phases; } }
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

    [Serializable]
    public class Phase
    {
        public int triggerTurn;
        public Vector2 position;
        public BiomeType type = default;

        public bool isLinkedBiomeEffect = false;
        public int effectID = -1;
    }

    [Serializable]
    public class TurnDeck
    {
        [Min(0)] public int turn = 0;
        public Deck deck = null;
    }
}
