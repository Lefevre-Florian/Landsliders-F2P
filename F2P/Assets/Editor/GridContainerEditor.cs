using com.isartdigital.f2p.gameplay.card;
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
        private GameObject[,] _Cards = new GameObject[3, 3];

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode) return;

            GridManager grid = (GridManager)target;

            if (grid.transform.childCount < 9)
            {
                foreach (Transform child in grid.transform) 
                    DestroyImmediate(child.gameObject);


                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                       _Cards[x,y] = Instantiate(grid._CardBackgroundPrefab, grid.transform);
                    }
                }
            }
            else 
            {
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        _Cards[x, y] = grid.transform.GetChild(x*3 + y).gameObject;
                    }
                }
            }


            grid._ScreenSizeInGameUnit = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
            grid._GridSize = grid._ScreenSizeInGameUnit * new Vector2(grid._GridSizePercent.x, grid._GridSizePercent.y);

            for (int x = -1; x <= 1; x++)
            {
                float lXPos = grid._GridSize.x * x;

                for (int y = -1; y <= 1; y++)
                {
                    float lYPos = grid._GridSize.y * y;
                    _Cards[x + 1, y + 1].transform.position = new Vector2(lXPos + grid._Offset.x, lYPos +grid._Offset.y);

                }
            }
        }

        

        private void OnSceneGUI()
        {
            
        }
    }
}

