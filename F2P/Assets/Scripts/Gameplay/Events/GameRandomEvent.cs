using System;
using System.Collections;
using com.isartdigital.f2p.gameplay.manager;
using Com.IsartDigital.F2P.Gameplay.Manager;
using UnityEngine;

namespace Com.IsartDigital.F2P.Gameplay.Events
{
    public class GameRandomEvent : MonoBehaviour
    {
        protected int _Priority;
        protected Vector2 _GridPosition;
        protected GridManager _GridManager = GridManager.GetInstance();

        protected virtual void Start()
        {
            GameManager.GetInstance().OnEffectPlayed += OnRandomEventTriggered;
            _GridPosition = GridManager.GetInstance().GetGridCoordinate(transform.position);
        }

        protected void OnRandomEventTriggered(int pPriority)
        {
            if (pPriority == _Priority)
            {
                PlayRandomEventEffect();
            }
        }

        protected virtual IEnumerator PlayRandomEventEffect()
        {
            return null;
        }

        protected void OnDestroy()
        {
            GameManager.GetInstance().OnEffectPlayed -= OnRandomEventTriggered;
            if (GameRandomEventsManager.GetInstance().GameEventCount > 0)
            {
                GameRandomEventsManager.GetInstance().GameEventCount -= 1;
            }
        }
    }
}
