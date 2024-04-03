using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CardEditorWindow : EditorWindow
{

    [MenuItem("Window/CardEditor")]
    public static void ShowWindow() 
    {
        GetWindow<CardEditorWindow>("CardEditor");
    }

    private void OnGUI()
    {
        
    }
}
