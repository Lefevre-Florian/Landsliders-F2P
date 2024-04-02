using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Parameters")]

    [SerializeField] private Vector2 _NumCard = Vector2.one * 3;
    [SerializeField] private Vector2 _GridSizePercent;
    [SerializeField] private Vector2 _Offset;

    [Header("GameObject")]

    [SerializeField] private GameObject _CardBackgroundPrefab;

    private Vector2 _ScreenSizeInGameUnit;
    private Vector2 _GridSize;

    private void Start()
    {
       if(_CardBackgroundPrefab == null) 
       {
            Debug.Log("GridManager : Champ serialisé _CardBackgroundPrefab non assigné");
            return;
       }

        _ScreenSizeInGameUnit = Camera.main.ViewportToWorldPoint(Vector2.one);
        _GridSize = _ScreenSizeInGameUnit * new Vector2(_GridSizePercent.x, _GridSizePercent.y) - CardContainer.staticSize;

        int xLoopMin = -(int)Math.Floor(_NumCard.x / 2);
        int xLoopMax = (int)(_NumCard.x - (int)Math.Ceiling(_NumCard.x / 2));

        int yLoopMin = -(int)Math.Floor(_NumCard.y / 2);
        int yLoopMax = (int)(_NumCard.y - (int)Math.Ceiling(_NumCard.y / 2));

        for (int x = xLoopMin; x <= xLoopMax; x++)
        {
            float lXPos = _GridSize.x * x / xLoopMax;

            for (int y = yLoopMin; y <= yLoopMax; y++)
            {
                float lYPos = _GridSize.y * y / yLoopMax;

                Instantiate(_CardBackgroundPrefab, new Vector3(lXPos + _Offset.y, lYPos + _Offset.y, 0), Quaternion.identity);

            }
        }
    }

    private void Update()
    {
    }
}

