using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;

using System.Collections.Generic;

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

        // Variables
        private IAnalyticsService _AnalyticsRemote = null;

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(this);
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
