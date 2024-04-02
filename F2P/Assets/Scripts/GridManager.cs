using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Parameters")]

    [SerializeField] private Vector2 _NumCard = Vector2.one * 3;
    [SerializeField] private GridAnchor _Anchor;

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

        _ScreenSizeInGameUnit = Camera.main.ViewportToWorldPoint(Vector2.one);

        int xLoopMin = -(int)Math.Floor(_NumCard.x / 2);
        int xLoopMax = (int)(_NumCard.x - (int)Math.Ceiling(_NumCard.x / 2));

        int yLoopMin = -(int)Math.Floor(_NumCard.y / 2);
        int yLoopMax = (int)(_NumCard.y - (int)Math.Ceiling(_NumCard.y / 2));

        for (int x = xLoopMin; x <= xLoopMax; x++)
        {
            float lXPos = _ScreenSizeInGameUnit.x * x / xLoopMax;

            for (int y = yLoopMin; y <= yLoopMax; y++)
            {
                float lYPos = _ScreenSizeInGameUnit.y * y / yLoopMax;

                Instantiate(_CardBackgroundPrefab, new Vector3(lXPos, lYPos, 0), Quaternion.identity);

            }
        }
    }

    private void Update()
    {
    }
}

[Serializable]
public struct GridAnchor 
{
    [SerializeField] private float _LeftAnchor;
    [SerializeField] private float _RightAnchor;
    [SerializeField] private float _DownAnchor;
    [SerializeField] private float _UpAnchor;
}
