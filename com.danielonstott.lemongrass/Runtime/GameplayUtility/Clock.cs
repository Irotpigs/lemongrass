//---------------------------------------------------------------------------//
// Name        - Clock.cs
// Author      - Daniel
// Project     - Lemongrass
// Description - Serves as an in-game timer and clock. Useful for games that
//  need to mimic clock timings such as a Five Nights at Freddy's
//---------------------------------------------------------------------------//
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Lemongrass
{
  /**
   * @brief A clock that can track timers and alarms. Scales to whatever in-game time
   *        time might be necessary.
   */
  public class Clock : MonoBehaviour
  {
    ////////////////////////////////////Variables//////////////////////////////////
    //-public--------------------------------------------------------------------//

    /**
     * @brief Returns the number of hours before the clock resets to 0
     */
    public int hourDelimiter { get { return bloodyOlEnglish24HourClock ? 24 : 12; } }

    //-private-------------------------------------------------------------------//

    // This variable is how time gets scaled. It is proportional to the number of minutes in an hour.
    // for example: 60 minutes behaves like a real clock whereas 30 minutes would be 2x faster. 1 minute
    // would mean 60x faster, etc.
    [SerializeField] protected float minutesPerHour = 60.0f;

    [SerializeField, Tooltip("True if this clock should go from 0-24 instead of 0-12.")]
    protected bool bloodyOlEnglish24HourClock = false;

    [SerializeField] private List<LemonTime.Timer> timers = new List<LemonTime.Timer>();
    [SerializeField] private List<LemonTime.Alarm> alarms = new List<LemonTime.Alarm>();

    private float partialSeconds = 0;
    protected int second = 0;
    protected int minute = 0;
    protected int hour = 0;

    ////////////////////////////////////Functions//////////////////////////////////
    //-public--------------------------------------------------------------------//

    /**
     * @brief Gets the time that this clock is tracking
     * @return A structure containing the hours, minutes, seconds that this clock
     *         has tracked.
     */
    public LemonTime.Time GetTime()
    {
      return new LemonTime.Time(second, minute, hour);
    }

    /**
     * @brief Manually set the number of seconds that this clock should read.
     * @param seconds The seconds to represent
     */
    public virtual void SetSecond(int seconds)
    {
      partialSeconds = 0;
      second = seconds % 60;
    }

    /**
     * @brief Manually set the number of minutes that this clock should read.
     * @param minutes The minutes to represent
     */
    public virtual void SetMinute(int minutes)
    {
      minute = minutes % 60;
    }

    /**
     * @brief Manually set the number of hours that this clock should read.
     * @param hours The hours to represent
     */
    public virtual void SetHour(int hours)
    {
      hour = hours % hourDelimiter;
    }

    //-private-------------------------------------------------------------------//
    private void Awake()
    {
      hour = System.DateTime.Now.Hour;
      minute = System.DateTime.Now.Minute;
      second = System.DateTime.Now.Second;

      SetHour(hour);
      SetMinute(minute);
      SetSecond(second);

      foreach (LemonTime.Timer timer in timers) timer.Awake();
    }

    protected virtual void Update()
    {
      partialSeconds += Time.deltaTime * (60.0f / minutesPerHour);

      bool secondPassed = partialSeconds >= 1.0;

      second += (int)partialSeconds;
      minute += (int)(second / 60.0f);
      hour += (int)(minute / 60.0f);

      second %= 60;
      minute %= 60;
      hour %= hourDelimiter;

      // A second has ticked since last update
      if (secondPassed)
      {
        partialSeconds = 0;
        // update timers and alarms each second
        timers.RemoveAll(item => item.Expired);
        alarms.RemoveAll(item => item.Expired);

        foreach (LemonTime.Timer timer in timers) timer.Update();
        foreach (LemonTime.Alarm alarm in alarms) alarm.Update(GetTime());
      }

    }
  }


  namespace LemonTime
  {
    [System.Serializable]
    /**
     * @brief Dont confuse this with regular time. This is a construct (unlike the other Time) for ChunkSum Clocks only!
     */
    public struct Time
    {
      ////////////////////////////////////Variables//////////////////////////////////
      //-public--------------------------------------------------------------------//
      /** 
       * @brief second The seconds represented in this time
       */
      public int Seconds;
      /** 
       * @brief minute The minutes represented in this time
       */
      public int Minutes;
      /** 
       * @brief hour The hours represented in this time
       */
      public int Hours;

      ////////////////////////////////////Functions//////////////////////////////////
      //-public--------------------------------------------------------------------//

      /**
       * @brief Construct a Time that represents the given values
       * @param second The seconds represented in this time. Not clamped to 60
       * @param minute The minutes represented in this time. Not clamped to 60
       * @param hour The hours represented in this time. Not clamped to hour delimiter
       */
      public Time(int second, int minute, int hour)
      {
        Seconds = second;
        Minutes = minute;
        Hours = hour;
      }

      /**
       * @brief Compare two time structures
       * @param lhs One of two times to compare
       * @param rhs One of two times to compare
       * @return True when the times are equal in all three fields.
       */
      static public bool operator ==(Time rhs, Time lhs)
      {
        return (rhs.Seconds == lhs.Seconds) &&
               (rhs.Minutes == lhs.Minutes) &&
               (rhs.Hours == lhs.Hours);
      }

      /**
       * @brief Compare two time structures
       * @param lhs One of two times to compare
       * @param rhs One of two times to compare
       * @return True when the times are not equal in one of three fields.
       */
      static public bool operator !=(Time rhs, Time lhs) { return !(rhs == lhs); }

      /////////////////////////// C# Comparison Overrides ////////////////////////////////
      public override bool Equals(object obj)
      {
        return base.Equals(obj);
      }
      public override int GetHashCode()
      {
        return base.GetHashCode();
      }
      ////////////////////////////////////////////////////////////////////////////////////
    }

    [System.Serializable]
    // This is a clock event that will go off after the given number of seconds
    public class Timer
    {
      ////////////////////////////////////Variables//////////////////////////////////
      //-public--------------------------------------------------------------------//

      /**
       * @brief The time that needs to pass since the clock is awoken before this timer is up
       */
      public uint TimeInSeconds = 0;

      /**
       * @brief This event is invoked every time this timer expires
       */
      public UnityEvent OnTimerExpired = new UnityEvent();

      /**
       * @brief Should this timer reset when it has expired
       */
      public bool Looping = false;

      /**
       * @brief Should this timer be invoked immediately upon the clock being awoken
       */
      public bool PlayOnAwake = false;

      /**
       * @brief If this timer has been expired
       */
      public bool Expired { get; private set; } = false;

      //-private-------------------------------------------------------------------//
      private uint internalTime = 0;

      ////////////////////////////////////Functions//////////////////////////////////
      //-public--------------------------------------------------------------------//

      /**
       * @brief Counts up the timer by one second. The timer will internally handle being invoked
       */
      public void Update()
      {
        ++internalTime;
        if (internalTime >= TimeInSeconds)
        {
          OnTimerExpired.Invoke();
          internalTime = 0;
          Expired = !Looping;
        }
      }

      /**
       * @brief Initializes the timer
       */
      public void Awake()
      {
        if (PlayOnAwake)
        {
          OnTimerExpired.Invoke();
          Expired = !Looping;
        }
      }
    }

    [System.Serializable]
    /**
     * @brief This is a clock event that will go off at the given time.
     */
    public class Alarm
    {
      ////////////////////////////////////Variables//////////////////////////////////
      //-public--------------------------------------------------------------------//

      /**
       * @brief The time that this alarm will expire at
       */
      public Time SetTime;

      /**
       * @brief This event is invoked every time this alarm expires
       */
      public UnityEvent OnTimerExpired = new UnityEvent();

      /**
       * @brief Should this alarm reset when it has expired and go off again the next time
       *        the set time is reached.
       */
      public bool Looping = false;

      /**
       * @brief If this alarm has been expired
       */
      public bool Expired { get; private set; } = false;

      ////////////////////////////////////Functions//////////////////////////////////
      //-public--------------------------------------------------------------------//

      /**
       * @brief Checks the given time against the alarm time and expires if needed.
       */
      public void Update(Time currentTime)
      {
        if (SetTime == currentTime)
        {
          OnTimerExpired.Invoke();
          Expired = !Looping;
        }
      }
    }
  }
}