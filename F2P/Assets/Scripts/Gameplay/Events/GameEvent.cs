using System;
using UnityEngine;

namespace Com.IsartDigital.F2P
{
    public enum GameEventType
    {
        ETHERALRIFT,
        DRAGONSLAIR,
        WITCH,
        WISP
    }


    public abstract class GameEvent : MonoBehaviour
    {

        void Start()
        {

        }

        public virtual void OnGameEventStarted()
        {

        }
    }
}
