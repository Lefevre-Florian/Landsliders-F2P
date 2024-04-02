using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Parameters")]

    [SerializeField] private Vector2 _NumCard = Vector2.one * 3;
    [SerializeField] private GridSize _GridSize;
    [SerializeField] private Vector3 _Offset;

    [Header("GameObject")]

    [SerializeField] private GameObject _CardBackgroundPrefab;

    private Vector2 _ScreenSizeInGameUnit;

    private void Start()
    {
       if(_CardBackgroundPrefab == null) 
       {
            Debug.Log("GridManager : Champ serialisé _CardBackgroundPrefab non assigné");
            return;
       }

        _ScreenSizeInGameUnit = Camera.main.ViewportToWorldPoint(Vector2.one) * new Vector2(_GridSize.width, _GridSize.height);

        int xLoopMin = -(int)Math.Floor(_NumCard.x / 2);
        int xLoopMax = (int)(_NumCard.x - (int)Math.Ceiling(_NumCard.x / 2));

        int yLoopMin = -(int)Math.Floor(_NumCard.y / 2);
        int yLoopMax = (int)(_NumCard.y - (int)Math.Ceiling(_NumCard.y / 2));

        for (int x = xLoopMin; x <= xLoopMax; x++)
        {
            float lXPos = _ScreenSizeInGameUnit.x * x / xLoopMax;

            for (int y = yLoopMin; y <= yLoopMax; y++)
            {
                float lYPos = _ScreenSizeInGameUnit.x * y / yLoopMax;

                Instantiate(_CardBackgroundPrefab, new Vector3(lXPos, lYPos, 0) + _Offset, Quaternion.identity);

            }
        }
    }

    private void Update()
    {
    }
}

[Serializable]
public struct GridSize
{
    [SerializeField] public float height;
    [SerializeField] public float width;
}
