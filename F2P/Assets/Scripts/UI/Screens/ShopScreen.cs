using Com.IsartDigital.F2P.Sound;

using System;
using System.Collections.Generic;

using UnityEngine;

// Author (CR) : Elias Dridi & Florian Lefevre
namespace Com.IsartDigital.F2P.UI
{
    public class ShopScreen : Screen
    {
        #region Tracking
        private const string TRACKER_NAME = "timeInShop";
        private const string TRACKER_TIME_PARAMETER = "timeInSecondMinute";
        #endregion

        [SerializeField] private MusicEmitter _Music = null;

        // Variables
        private DateTime _StartTime = default;

        private DataTracker _Tracker = null;

        private void Start()
        {
            _Tracker = DataTracker.GetInstance();
        }

        public override void Open()
        {
            _StartTime = DateTime.UtcNow;
            base.Open();

            if (_Music != null)
                _Music.SetImmediateFade(1);
        }

        public override void Close()
        {
            if(_Tracker != null)
            {
                TimeSpan lDuration = (DateTime.UtcNow - _StartTime).Duration();
                _Tracker.SendAnalytics(TRACKER_NAME, new Dictionary<string, object>() { { TRACKER_TIME_PARAMETER, lDuration.Minutes + ":" + lDuration.Seconds } });
            }
            base.Close();
        }

        private void OnDestroy()
        {
            _Tracker = null;
        }
    }
}

