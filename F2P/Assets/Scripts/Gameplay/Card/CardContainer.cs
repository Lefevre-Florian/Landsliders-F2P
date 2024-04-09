using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace com.isartdigital.f2p.gameplay.card 
{
    public enum CardTypes {
        NONE,
        PLAINE
    }

    [RequireComponent(typeof(BoxCollider2D))]
    public class CardContainer : MonoBehaviour
    {
        [HideInInspector] public BoxCollider2D boxCollider;

        [SerializeField] private List<ScriptableCard> allCardsParameters = new List<ScriptableCard>();

        private ScriptableCard myCard;

        [HideInInspector] public float size;
        [HideInInspector] public float cardRatio = 1.39f;

        private void Start()
        {

        }

        public void Init() 
        {
            myCard = allCardsParameters[Mathf.FloorToInt(UnityEngine.Random.Range(0, allCardsParameters.Count))];
            foreach (MonoScript script in myCard.Scripts)
                gameObject.AddComponent(script.GetClass());
        }

        public BoxCollider2D GetBoxCollider() 
        {
            if(boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();

            return boxCollider;


        }

    }
}
