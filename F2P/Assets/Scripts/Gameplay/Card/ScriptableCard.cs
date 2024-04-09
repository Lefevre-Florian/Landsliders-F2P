using com.isartdigital.f2p.gameplay.card;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Card Type", menuName = "ScriptableObject/CardTypes")]
public class ScriptableCard : ScriptableObject
{
    public CardTypes CardTypes;

    public string Name;
    public string Description;

    public List<MonoScript> Scripts;
}
