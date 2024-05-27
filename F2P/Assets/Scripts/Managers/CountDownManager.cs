using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CountDown : MonoBehaviour
{
    public TextMeshProUGUI countdownText;

    void Update()
    {
        TimeSpan timeRemaining = GetTimeUntilMidnight();
        countdownText.text = FormatTimeSpan(timeRemaining);
    }

    TimeSpan GetTimeUntilMidnight()
    {
        DateTime now = DateTime.Now;
        DateTime midnight = now.Date.AddDays(1); // Tomorrow's midnight
        return midnight - now;
    }

    string FormatTimeSpan(TimeSpan timeSpan)
    {
        return string.Format("{0:D2}:{1:D2}:{2:D2}",
            timeSpan.Hours,
            timeSpan.Minutes,
            timeSpan.Seconds);
    }

}
