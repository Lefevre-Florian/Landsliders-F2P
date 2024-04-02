using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardContainer))]
public class CardContainerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CardContainer cardContainer = (CardContainer)target;

        cardContainer.GetBoxCollider().size = cardContainer.size;

        EditorUtility.SetDirty(cardContainer.gameObject);
    }
}
