using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.isartdigital.f2p.gameplay.card 
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CardContainer : MonoBehaviour
    {
        [HideInInspector] public BoxCollider2D boxCollider;

        [SerializeField] public float size;
        [SerializeField] public float cardRatio = 1.39f;


        public static float staticSize = 1;

        private void Awake()
        {
            staticSize = 1;
        }

        public BoxCollider2D GetBoxCollider() 
        {
            if(boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();

            return boxCollider;
        }

    }
}
