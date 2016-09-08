using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class DateAndTime : MonoSingleton<DateAndTime> 
{
  public Text TimeText;

  protected override void Init()
  {
    base.Init ();
  }

  InGameDateTime _inGameDateTime = new InGameDateTime();

  string _inGameTimeAsString = string.Empty;

  int _inGameTimeSeconds = 0;

  float _timer = 0.0f;
  void Update()
  {
    if (_timer > GlobalConstants.InGameSecondTick)
    {      
      UpdateInGameTime();
      _timer = 0.0f;
    }

    _timer += Time.smoothDeltaTime * GlobalConstants.InGameTimeFlowSpeed;

    _inGameTimeAsString = string.Format("{0}:{1}:{2}", (_inGameTimeSeconds / 3600) % 24, (_inGameTimeSeconds / 60) % 60, _inGameTimeSeconds % 60); 

    TimeText.text = string.Format("{0}\n{1}\n{2}", _inGameDateTime.ToString(), _inGameTimeAsString, _inGameTimeSeconds);
  }

  void UpdateInGameTime()
  {
    _inGameTimeSeconds++;

    if (_inGameTimeSeconds > GlobalConstants.InGameDayNightLength)
    {
      _inGameTimeSeconds = 0;

      _inGameDateTime.IncrementDay();
    }
  }
}

public class InGameDateTime
{
  public int Day = 7;
  public int Month = 1;
  public int Year = 988;

  public InGameDateTime()
  {
    Day = 7;
    Month = 1;
    Year = 988;
  }

  public InGameDateTime(int d, int m, int y)
  {
    Day = d;
    Month = m;
    Year = y;
  }

  public void IncrementDay()
  {
    Day++;

    if (Day > GlobalConstants.InGameDaysInMonth)
    {
      Day = 1;
      Month++;
    }

    if (Month > GlobalConstants.InGameMonthsInYear)
    {
      Month = 1;
      Year++;
    }
  }

  public override string ToString()
  {
    return string.Format("{0} of {1} {2}", Day, GlobalConstants.InGameMonthsNames[Month - 1], Year);
  }
}
