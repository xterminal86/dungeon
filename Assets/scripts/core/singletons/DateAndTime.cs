using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class DateAndTime : MonoSingleton<DateAndTime> 
{
  public Text TimeText;

  int _inGameTime = 0;
  public int InGameTime
  {
    get { return _inGameTime; }
    set { _inGameTime = value; }
  }

  string _daytimeString = string.Empty;
  public string DaytimeString
  {
    get { return _daytimeString; }
  }

  public bool FreezeTime = false;

  float _timerCondition = 0.0f;
  protected override void Init()
  {
    base.Init();

    // Assuming, that deltaTime is identical, after one second we will get _timer = (FPS * deltaTime) / FPS.
    // It should be close to 1.0f.
    // We would like to update in-game time TicksPerSecond times per second.
    // To do that we divide 1 by TicksPerSecond to get limit of deltaTime accumulation.
    _timerCondition = 1.0f / (float)GlobalConstants.TicksPerSecond;
  }

  InGameDateTime _inGameDateTime = new InGameDateTime();

  // If we use time in Update() of SunController and here,
  // it will become unsynchronized. That's why we make one in-game
  // clock for synchronization.

  bool _wasTick = false;
  public bool WasTick
  {
    get { return _wasTick; }
  }

  float _timer = 0.0f;
  void Update()
  {
    if (FreezeTime)
    {
      return;
    }

    _wasTick = false;

    if (_timer > _timerCondition)
    {    
      _wasTick = true;

      _inGameTime++;
      _timer = 0.0f;

      if (_inGameTime > GlobalConstants.InGameDayNightLength)
      {
        _inGameTime = 0;
        _inGameDateTime.IncrementDay();
      }

      UpdateDaytimeString();
    }

    _timer += Time.deltaTime;

    TimeText.text = string.Format("{0}\n{1} ({2})", _inGameDateTime.ToString(), _inGameTime, _daytimeString);
  }

  void UpdateDaytimeString()
  {
    //if (InGameTime < GlobalConstants.InGameDawnEndSeconds)
    if (InGameTime > GlobalConstants.DawnStartTime && InGameTime < GlobalConstants.DawnEndTime)
    {
      _daytimeString = "Dawn";
    }
    //else if (InGameTime > GlobalConstants.InGameDawnEndSeconds && InGameTime < GlobalConstants.InGameDuskStartSeconds)
    else if (InGameTime > GlobalConstants.DawnStartTime && InGameTime < GlobalConstants.DuskStartTime)
    {
      _daytimeString = "Daytime";
    }
    //else if (InGameTime > GlobalConstants.InGameDuskStartSeconds && InGameTime < GlobalConstants.InGameNightStartSeconds)
    else if (InGameTime > GlobalConstants.DuskStartTime && InGameTime < GlobalConstants.DuskEndTime)
    {
      _daytimeString = "Dusk";
    }
    //else if (InGameTime > GlobalConstants.InGameNightStartSeconds)
    else if (InGameTime > GlobalConstants.DuskEndTime)
    {
      _daytimeString = "Night";
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
