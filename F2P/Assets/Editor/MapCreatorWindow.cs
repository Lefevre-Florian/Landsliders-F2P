using com.isartdigital.f2p.editor;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEditor;
using UnityEngine;

public class MapCreatorWindow : EditorWindow
{
    [MenuItem("Window/MapCreator")]
    public static void ShowWindow()
    {
        GetWindow<MapCreatorWindow>("MapCreator");
    }

    public GameObject[,] _Cards = new GameObject[3,3];
    private string assetName;

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        for (int x = 0; x < 3; x++)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(300));

            for (int y = 0; y < 3; y++)
            {
                EditorGUILayout.BeginVertical();

                _Cards[x,y] = (GameObject)EditorGUILayout.ObjectField((Object)_Cards[x,y], typeof(Object), false, GUILayout.Width(100), GUILayout.Height(100));
                
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();
            
        }

        assetName = EditorGUILayout.TextField(assetName, GUILayout.Width(300));
        if (GUILayout.Button("SaveMap", GUILayout.Width(300)))
        {
            SaveMap();

        }

        EditorGUILayout.EndVertical();

    }

    private void SaveMap()
    {
        if (assetName == "")
        {
            Debug.Log("Nom non Valide");
            return;
        }

        MapScriptableObj lMap = AssetDatabase.LoadAssetAtPath<MapScriptableObj>("Assets/Ressources/Maps/" + assetName);

        if (lMap) Debug.Log("Nom déjà utilisé");
        lMap = CreateInstance<MapScriptableObj>();

        AssetDatabase.CreateAsset(lMap, "Assets/Ressources/Maps/" + assetName + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        SerializedObject SerializedMap = new SerializedObject(lMap);
        SerializedMap.Update();

        for (int x = 0; x < 3; x++)
        {
            SerializedProperty SaveMap = SerializedMap.FindProperty("Map" + x);
            for (int y = 0; y < 3; y++)
            {
                SerializedProperty lArrayElement = SaveMap.GetArrayElementAtIndex(y);
                lArrayElement.objectReferenceValue = _Cards[x, y];
            }
        }

        SerializedMap.ApplyModifiedProperties();
    }
}
