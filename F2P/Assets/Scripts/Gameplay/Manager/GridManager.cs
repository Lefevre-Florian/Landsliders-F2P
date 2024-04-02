using com.isartdigital.f2p.gameplay.card;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace com.isartdigital.f2p.gameplay.manager 
{
    public class GridManager : MonoBehaviour
    {
        private static GridManager instance;

        public static GridManager GetInstance() 
        {
            if(instance == null) instance = new GridManager(); 
            return instance;
        }

        private void Awake()
        {
            if(instance != null) 
            {
                DestroyImmediate(this);
                return;
            }
            instance = this;
        }

        [Header("Grid Parameters")]

        [HideInInspector] public Vector2 _NumCard = Vector2.one * 3;
        [SerializeField] public Vector2 _GridSizePercent;
        [SerializeField] public Vector2 _Offset;

        [Header("GameObject")]

        [SerializeField] public GameObject _CardBackgroundPrefab;

        [HideInInspector] public Vector2 _ScreenSizeInGameUnit;
        [HideInInspector] public Vector2 _GridSize;

        [HideInInspector] public GameObject[,] _Cards;

        private void Start()
        {
            _Cards = new GameObject[(int)_NumCard.x, (int)_NumCard.y];

            foreach (Transform child in transform) 
            {
                Destroy(child.gameObject);
            }

           if(_CardBackgroundPrefab == null) 
           {
                Debug.Log("GridManager : Champ serialisé _CardBackgroundPrefab non assigné");
                return;
           }

            _ScreenSizeInGameUnit = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
            _GridSize = _ScreenSizeInGameUnit * new Vector2(_GridSizePercent.x, _GridSizePercent.y) - new Vector2(CardContainer.staticSize, CardContainer.staticSize * 1.39f) / 2;

            int lXArrayIndex = 0;
            for (int x = -1; x <= 1; x++)
            {
                float lXPos = _GridSize.x * x;

                int lYArrayIndex = 0;
                for (int y = -1; y <= 1; y++)
                {
                    float lYPos = _GridSize.y * y;

                    _Cards[lXArrayIndex, lYArrayIndex] = Instantiate(_CardBackgroundPrefab, new Vector3(lXPos + _Offset.x, lYPos + _Offset.y, 0), Quaternion.identity, transform);
                    lYArrayIndex++;
                }
                lXArrayIndex++;
            }
        }

        public Vector2 GetIndexCoordonate(int pX, int pY) 
        {
            if(pX > 2 || pX < 0 || pY > 2 || pY < 0) 
            {
                Debug.Log("Les parametres rentré sont : pX = " + pX.ToString() + ", pY = " + pY.ToString() + "Cependant la fonction a pour intervale x[0,2] et y[0,2]");
                return Vector2.zero;
            }

            return new Vector2(_GridSize.x * (pX - 1) + _Offset.x, _GridSize.y * (pY - 1) + _Offset.y);
        }

        private void OnDestroy()
        {
            if (instance == this) instance = null;
        }
    }
}

