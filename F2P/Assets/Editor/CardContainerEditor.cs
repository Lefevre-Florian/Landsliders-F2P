using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using com.isartdigital.f2p.gameplay.card;

namespace com.isartdigital.f2p.editor 
{
    [CustomEditor(typeof(CardContainer))]
    public class CardContainerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CardContainer cardContainer = (CardContainer)target;

            Camera cam = Camera.main;
            float height = cam.orthographicSize * 2;
            float width = cam.aspect * height;
            float lXSize = width * cardContainer.size / 10;

            cardContainer.GetBoxCollider().size = new Vector2(lXSize, lXSize * cardContainer.cardRatio);
            CardContainer.staticSize = cardContainer.size;
            EditorUtility.SetDirty(cardContainer.gameObject);
        }
    }
}
