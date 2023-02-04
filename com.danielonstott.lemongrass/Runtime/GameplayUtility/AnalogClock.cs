//---------------------------------------------------------------------------//
// Name        - AnalogClock.cs
// Author      - Daniel
// Project     - Lemongrass
// Description - Extends the clock into an analog clock prop. If you managed
//  to make a game without one of these then pray that I do not meet you face
//  to face someday.
//---------------------------------------------------------------------------//
using UnityEngine;

namespace Lemongrass
{
  /**
   * @brief Just make sure your game uses one of these. And turn the 3D audio *up*
   */
  public class AnalogClock : Clock
  {
    ////////////////////////////////////Variables//////////////////////////////////
    //-private-------------------------------------------------------------------//
    [SerializeField, Tooltip("The clock expects pivots to rotate around this object's blue axis")]
    private Transform minuteHandPivot, hourHandPivot, secondHandPivot;

    ////////////////////////////////////Functions//////////////////////////////////
    //-public--------------------------------------------------------------------//

    /**
     * @brief Manually set the number of seconds that this clock should read.
     * @param seconds The seconds to represent
     */
    public override void SetSecond(int seconds)
    {
      base.SetSecond(seconds);
      UpdateHands();
    }

    /**
     * @brief Manually set the number of minutes that this clock should read.
     * @param minutes The minutes to represent
     */
    public override void SetMinute(int minutes)
    {
      base.SetMinute(minutes);
      UpdateHands();
    }

    /**
     * @brief Manually set the number of hours that this clock should read.
     * @param hours The hours to represent
     */
    public override void SetHour(int hours)
    {
      base.SetHour(hours);
      UpdateHands();
    }
    //-private-------------------------------------------------------------------//

    private void Start()
    {
      UpdateHands();
    }

    protected override void Update()
    {
      base.Update();
      UpdateHands();
    }

    private void UpdateHands()
    {
      secondHandPivot.rotation = Quaternion.AngleAxis(second * 6.0f, transform.forward) * transform.rotation;
      minuteHandPivot.rotation = Quaternion.AngleAxis(minute * 6.0f, transform.forward) * transform.rotation;
      hourHandPivot.rotation = Quaternion.AngleAxis(hour * (360.0f / hourDelimiter), transform.forward) * transform.rotation;
    }
  }
}
