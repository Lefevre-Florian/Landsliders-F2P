using com.isartdigital.f2p.gameplay.manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.isartdigital.f2p.editor 
{
    [CustomEditor(typeof(GridManager))]
    public class GridContainerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode) return;

            GridManager grid = (GridManager)target;

            foreach(Transform child in grid.transform) 
            {
                DestroyImmediate(child.gameObject);
            }


            grid._ScreenSizeInGameUnit = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
            grid._GridSize = grid._ScreenSizeInGameUnit * new Vector2(grid._GridSizePercent.x, grid._GridSizePercent.y) -new Vector2(CardContainer.staticSize, CardContainer.staticSize * 1.39f) / 2;

            for (int x = -1; x <= 1; x++)
            {
                float lXPos = grid._GridSize.x * x;

                for (int y = -1; y <= 1; y++)
                {
                    float lYPos = grid._GridSize.y * y;

                    Instantiate(grid._CardBackgroundPrefab, new Vector3(lXPos + grid._Offset.x, lYPos + grid._Offset.y, 0), Quaternion.identity, grid.transform);

                }
            }
        }
    }
}

