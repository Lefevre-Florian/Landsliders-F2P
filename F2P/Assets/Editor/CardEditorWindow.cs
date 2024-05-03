using UnityEngine;
using UnityEditor;
using com.isartdigital.f2p.gameplay.card;
using com.isartdigital.f2p.gameplay.manager;
using Codice.CM.Common.Replication;

namespace com.isartdigital.f2p.editor 
{
    public class CardEditorWindow : EditorWindow
    {

        private float _ScaleValue = 1;

        [MenuItem("Window/CardEditor")]
        public static void ShowWindow()
        {
            GetWindow<CardEditorWindow>("CardEditor");
        }

        private static CardShapeData data;

        [InitializeOnLoadMethod]
        private static void OnLoad()
        {
            if (!data)
            {
                data = AssetDatabase.LoadAssetAtPath<CardShapeData>("Assets/Editor/CardShapeData.asset");

                if (data) return;

                data = CreateInstance<CardShapeData>();

                AssetDatabase.CreateAsset(data, "Assets/Editor/CardShapeData.asset");
                AssetDatabase.Refresh();
            }
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(0, 0, 100, 30), "Card Size");

            SerializedObject serializedObject = new SerializedObject(data);
            serializedObject.Update();

            var lSize = serializedObject.FindProperty("size");
            var lRatio = serializedObject.FindProperty("ratio");

            lSize.floatValue = EditorGUI.FloatField(new Rect(190, 7, 100, 20), lSize.floatValue);
            lSize.floatValue = GUI.HorizontalSlider(new Rect(75, 7, 100, 30), lSize.floatValue, 0, 4);

            GUI.Label(new Rect(0, 30, 100, 30), "Card Ratio");

            lRatio.floatValue = EditorGUI.FloatField(new Rect(190, 37, 100, 20), lRatio.floatValue);
            lRatio.floatValue = GUI.HorizontalSlider(new Rect(75, 37, 100, 30), lRatio.floatValue, 0.5f, 2);

            float _RectHeight = 100 * (lSize.floatValue + .5f);
            float _RectWidth = _RectHeight / lRatio.floatValue;
            EditorGUI.DrawRect(new Rect(10, 60, _RectWidth, _RectHeight), Color.green);

            CardContainer[] cardContainers = Resources.FindObjectsOfTypeAll<CardContainer>();

            Camera cam = Camera.main;
            float height = cam.orthographicSize * 2;
            float width = cam.aspect * height;
            foreach (CardContainer cardContainer in cardContainers)
            {
                float lXSize = width * cardContainer.size / 10;
                cardContainer.size = lSize.floatValue;
                cardContainer.cardRatio = lRatio.floatValue;

                cardContainer.GetBoxCollider().size = new Vector2(lXSize, lXSize * cardContainer.cardRatio);
                EditorUtility.SetDirty(cardContainer.gameObject);
            }

            GridManager grid = Resources.FindObjectsOfTypeAll<GridManager>()[0];
            grid._CardRatio = lRatio.floatValue;
            grid._CardSize = lSize.floatValue;

            serializedObject.ApplyModifiedProperties();
        }
    }
}

