//---------------------------------------------------------------------------//
// Name        - LemongrassMath.cs
// Author      - Daniel Onstott
// Project     - Lemongrass
// Description - A collection of utility functions that can be used for
//  miscellaneous math stuff
//---------------------------------------------------------------------------//
using System;
using UnityEngine;

namespace Lemongrass
{
  /**
   * @brief A collection of utility functions that are used for math.
   */
  public static class LemonMath
  {
    /**
     * @brief Maps a value in [-180, 180] to [0, 360]
     * @param angle An euler angle value in [-180, 180]
     * @return The same angle in the range [0, 360]
     */
    public static float EulerTo360(float angle)
    {
      if (angle >= 0) return angle;
      return 360.0f + angle;
    }

    /**
     * @brief Rounds the given value to the nearest increment of the given increment
     * @param value The value to be rounded
     * @param increment The increment to round the value to. NOTE that this is an unsigned value
     * @return The value having been rounded
     */
    public static int RoundToValue(int value, UInt16 increment)
    {
      // we will convert back to a negative number after the math is done
      bool wasNegative = value < 0;
      value = Mathf.Abs(value);

      uint segment = 0;
      while (value > increment)
      {
        segment += increment;
        value -= increment;
      }

      if (value >= (increment / 2)) value = increment;
      else value = 0;

      int returnVal = (int)(segment + value);
      return wasNegative ? -returnVal : returnVal;
    }
  }
}
