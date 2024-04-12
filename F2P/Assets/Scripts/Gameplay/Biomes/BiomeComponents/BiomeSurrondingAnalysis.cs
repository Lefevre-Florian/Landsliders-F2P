using com.isartdigital.f2p.gameplay.manager;

using System.Collections.Generic;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    public class BiomeSurrondingAnalysis : MonoBehaviour
    {
        private enum GridAngle
        {
            AllDirection = 8,
            CrossDirection = 4,
            NextDirection = 2
        }

        [Header("Design")]
        [SerializeField] private GridAngle _GridDirection = GridAngle.CrossDirection;

        // Variables
        private GridManager _GridManager = null;

        private Vector2 _GridPosition = Vector2.zero;

        private void Start()
        {
            _GridManager = GridManager.GetInstance();

            Vector2 lLength = _GridManager._NumCard;
            GameObject[,] lGrid = _GridManager._Cards;

            for (int i = 0; i < lLength.x; i++)
            {
                for (int j = 0; j < lLength.y; j++)
                {
                    if (lGrid[i, j].transform == transform.parent)
                    {
                        _GridPosition = new Vector2(i, j);
                        break;
                    }
                }
            }
        }

        public GameObject[] GetSurrounding()
        {
            List<GameObject> lSurroundingBiomes = new List<GameObject>();
            Vector2 lSamplePosition = default;

            int lLength = (int)_GridDirection;
            float lAngle = ((Mathf.PI * 2f) / (int)_GridDirection) * Mathf.Rad2Deg;

            for (int i = 0; i < lLength; i++)
            {
                lSamplePosition = _GridPosition + (Vector2)(Quaternion.AngleAxis(lAngle * i, Vector3.forward) * Vector3.up);
                lSamplePosition.x = Mathf.RoundToInt(lSamplePosition.x);
                lSamplePosition.y = Mathf.RoundToInt(lSamplePosition.y);

                Debug.Log(lSamplePosition);

                // Check out of bound
                if (lSamplePosition.x >= 0f
                    && lSamplePosition.x < _GridManager._NumCard.x
                    && lSamplePosition.y >= 0f
                    && lSamplePosition.y < _GridManager._NumCard.y)
                {
                    lSurroundingBiomes.Add(_GridManager.GetCardByGridCoordinate(lSamplePosition));
                }
                    
            }
            return lSurroundingBiomes.ToArray();
        }

        public void Echo() => Debug.Log("ok");

        private void OnDestroy() => _GridManager = null;
    }
}
