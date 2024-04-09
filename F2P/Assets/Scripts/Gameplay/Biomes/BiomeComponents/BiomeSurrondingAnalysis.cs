using com.isartdigital.f2p.gameplay.manager;

using System.Collections.Generic;

using UnityEngine;

// Author (CR) : Lefevre Florian
namespace Com.IsartDigital.F2P.Biomes
{
    public class BiomeSurrondingAnalysis : MonoBehaviour
    {
        private const float NB_SAMPLE = 4f;

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

            float lQuaterOfCircle = Mathf.PI / 2f * Mathf.Rad2Deg;
            Vector2 lSamplePosition = default;

            for (int i = 0; i < NB_SAMPLE; i++)
            {
                lSamplePosition = _GridPosition + (Vector2)(Quaternion.AngleAxis(lQuaterOfCircle * i, Vector3.forward) * Vector3.up);

                // Check out of bound
                if (lSamplePosition.x >= 0f
                    && lSamplePosition.x < _GridManager._NumCard.x
                    && lSamplePosition.y >= 0f
                    && lSamplePosition.y < _GridManager._NumCard.y)
                    lSurroundingBiomes.Add(_GridManager.GetCardByGridCoordinate(lSamplePosition));
            }
            return lSurroundingBiomes.ToArray();
        }

        private void OnDestroy() => _GridManager = null;
    }
}
