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
    public static int RoundToValue(int value, ushort increment)
    {
      if (increment == 0) return 0;
      int rounding = value + increment / 2;
      return rounding - rounding % increment;
    }

    /**
     * @brief Rounds the given value to the nearest increment of the given increment
     * @param value The value to be rounded
     * @param increment The increment to round the value to. NOTE that this is an unsigned value
     * @return The value having been rounded
     */
    public static float RoundToValue(float value, ushort increment)
    {
      if (increment == 0) return 0;
      float rounding = (float)Math.Round(value) + increment / 2;
      return rounding - rounding % increment;
    }

    /**
     * @brief Rounds the given value to the nearest increment of the given increment
     * @param value The value to be rounded
     * @param increment The increment to round the value to. NOTE that this is an unsigned value
     * @return The value having been rounded
     */
    public static double RoundToValue(double value, ushort increment)
    {
      if (increment == 0) return 0;
      double rounding = Math.Round(value) + increment / 2;
      return rounding - rounding % increment;
    }

    /**
     * @brief Rounds the given vector to the nearest increment of the given increment for all components of the vector
     * @param value The vector to be rounded
     * @param increment The increment to round the vectors components to. NOTE that this is an unsigned value
     * @return The vector having had the components rounded
     */
    public static Vector3 RoundToValue(Vector3 value, ushort increment)
    {
      value.x = RoundToValue(value.x, increment);
      value.y = RoundToValue(value.y, increment);
      value.z = RoundToValue(value.z, increment);
      return value;
    }
    /**
     * @brief Rounds the given vector to the nearest increment of the given increment for all components of the vector
     * @param value The vector to be rounded
     * @param increment The increment to round the vectors components to. NOTE that this is an unsigned value
     * @return The vector having had the components rounded
     */
    public static Vector3Int RoundToValue(Vector3Int value, ushort increment)
    {
      value.x = RoundToValue(value.x, increment);
      value.y = RoundToValue(value.y, increment);
      value.z = RoundToValue(value.z, increment);
      return value;
    }
  }
}
