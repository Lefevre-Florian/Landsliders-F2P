using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;

using System;
using System.Collections.Generic;

using Com.IsartDigital.F2P.FileSystem;

namespace Com.IsartDigital.F2P
{
    public class DataTracker : MonoBehaviour
    {
        #region Singleton
        private static DataTracker _Instance = null;

        public static DataTracker GetInstance()
        {
            if(_Instance == null) 
				_Instance = new DataTracker();
            return _Instance;
        }

        private DataTracker() : base() {}
        #endregion

        #region Tracking
        private const string TRACKER_TOTAL_PLAYTIME_NAME = "globalPlayTime";
        private const string TRACKER_TOTAL_GAME_NAME = "numberGameLaunchDuringSession";
        private const string TRACKER_SESSION_PLAYTIME_NAME = "sessionDuration";

        private const string TRACKER_HOUR_MINUTE_PARAMETER = "timeInHourMinute";
        private const string TRACKER_MINUTE_SECOND_PARAMETER = "timeInSecondMinute";
        private const string TRACKER_NUMBER_GAME_STARTED_PARAMETER = "numberOfGame";
        #endregion 

        // Variables
        private IAnalyticsService _AnalyticsRemote = null;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        private async void Start()
        {
            await UnityServices.InitializeAsync();
            Connect();
        }

        private void Connect()
        {
            _AnalyticsRemote = AnalyticsService.Instance;
            _AnalyticsRemote.StartDataCollection();
        }

        public void SendAnalytics(string pEventName, Dictionary<string, object> pParameters)
        {
            CustomEvent lEvent = new CustomEvent(pEventName);
            foreach (KeyValuePair<string, object> lItem in pParameters)
                lEvent.Add(lItem.Key, lItem.Value);

            _AnalyticsRemote.RecordEvent(lEvent);
            _AnalyticsRemote.Flush();
        }

        private void OnApplicationFocus(bool focus)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            if (!focus)
                SessionAnalytics();
            #endif
        }

        private void OnApplicationQuit() => SessionAnalytics();

        private void SessionAnalytics()
        {
            // First tracker : session playtime
            TimeSpan lDuration = (DateTime.UtcNow - Save.data.startTime).Duration();
            SendAnalytics(TRACKER_SESSION_PLAYTIME_NAME, 
                          new Dictionary<string, object>() { { TRACKER_MINUTE_SECOND_PARAMETER, lDuration.Minutes + ":" + lDuration.Seconds } });

            // Second tracker : total playtime
            TimeSpan lPlaytime = Save.data.totalPlaytime + lDuration;
            Save.data.totalPlaytime = lPlaytime;

            DatabaseManager.GetInstance().WriteDataToSaveFile();
            SendAnalytics(TRACKER_TOTAL_PLAYTIME_NAME,
                          new Dictionary<string, object>() { { TRACKER_HOUR_MINUTE_PARAMETER, lPlaytime.Hours + ":" + lPlaytime.Minutes } });

            // Third tracker : total play in this session
            SendAnalytics(TRACKER_TOTAL_GAME_NAME,
                          new Dictionary<string, object>() { { TRACKER_NUMBER_GAME_STARTED_PARAMETER, Save.data.totalGame } });
        }

        private void OnDestroy()
        {
            if (_Instance == this)
            {
                _AnalyticsRemote.Flush();
                _AnalyticsRemote = null;

                _Instance = null;
            }
        }
    }
}
