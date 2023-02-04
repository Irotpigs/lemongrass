//---------------------------------------------------------------------------//
// Name        - DigitalClock.cs
// Author      - Daniel
// Project     - Lemongrass
// Description - Extends the clock into a digital clock prop.
//---------------------------------------------------------------------------//
using TMPro;
using UnityEngine;

namespace Lemongrass
{
  /**
   * @brief Extends the clock into a digital clock prop. Useful for the preschoolers playing
   *        that can't read an analog clock
   */
  public class DigitalClock : Clock
  {
    ////////////////////////////////////Variables//////////////////////////////////
    //-private-------------------------------------------------------------------//
    [SerializeField] private TextMeshPro clockText;
    [SerializeField] private bool showHours = false, showMinutes = false, showSeconds = false;

    ////////////////////////////////////Functions//////////////////////////////////
    //-public--------------------------------------------------------------------//

    /**
     * @brief Manually set the number of seconds that this clock should read.
     * @param seconds The seconds to represent
     */
    public override void SetSecond(int seconds)
    {
      base.SetSecond(seconds);
      UpdateText();
    }

    /**
     * @brief Manually set the number of minutes that this clock should read.
     * @param minutes The minutes to represent
     */
    public override void SetMinute(int minutes)
    {
      base.SetMinute(minutes);
      UpdateText();
    }

    /**
     * @brief Manually set the number of hours that this clock should read.
     * @param hours The hours to represent
     */
    public override void SetHour(int hours)
    {
      base.SetHour(hours);
      UpdateText();
    }

    //-private-------------------------------------------------------------------//
    private void Start()
    {
      UpdateText();
    }

    protected override void Update()
    {
      base.Update();
      UpdateText();
    }

    private void UpdateText()
    {
      string currentTime = "";

      if (showHours) currentTime += hour.ToString("D2") + ':';
      if (showMinutes) currentTime += minute.ToString("D2");
      if (showSeconds && !showMinutes && showHours) currentTime += "00"; // you are bad at clocks if you hit this
      if (showSeconds) currentTime += ':' + second.ToString("D2");

      clockText.text = currentTime;
    }
  }
}
