using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.isartdigital.f2p.gameplay.card 
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CardContainer : MonoBehaviour
    {
        [HideInInspector] public BoxCollider2D boxCollider;

        [HideInInspector] public float size;
        [HideInInspector] public float cardRatio = 1.39f;

        public BoxCollider2D GetBoxCollider() 
        {
            if(boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();

            return boxCollider;


        }

    }
}
