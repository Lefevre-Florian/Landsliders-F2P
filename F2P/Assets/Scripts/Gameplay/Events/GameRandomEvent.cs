using System;
using System.Collections;
using com.isartdigital.f2p.gameplay.manager;
using Com.IsartDigital.F2P.Gameplay.Manager;
using UnityEngine;

// Author: Dorian Husson
namespace Com.IsartDigital.F2P.Gameplay.Events
{
    public class GameRandomEvent : MonoBehaviour
    {
        [SerializeField][Min(0)] protected int _Priority;
        protected Vector2 _GridPosition;

        protected GameManager _GameManager = GameManager.GetInstance();
        protected GridManager _GridManager = GridManager.GetInstance();
        protected HandManager _HandManager = HandManager.GetInstance();
        protected Player _Player = Player.GetInstance();

        protected virtual void Start()
        {
            _GameManager.OnEffectPlayed += OnRandomEventTriggered;
            _GridPosition = GridManager.GetInstance().GetGridCoordinate(transform.position);
        }

        protected void OnRandomEventTriggered(int pPriority)
        {
            if (pPriority == _Priority)
            {
                PlayRandomEventEffect();
            }
        }

        protected virtual void PlayRandomEventEffect()
        {

        }

        protected void OnDestroy()
        {
            _GameManager.OnEffectPlayed -= OnRandomEventTriggered;

            if (GameRandomEventsManager.GetInstance().GameEventCount > 0)
            {
                GameRandomEventsManager.GetInstance().GameEventCount -= 1;
            }
        }
    }
}
