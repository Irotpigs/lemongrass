//---------------------------------------------------------------------------//
// Name        - LemongrassVectors.cs
// Author      - Daniel Onstott
// Project     - Lemongrass
// Description - A collection of utility functions that can be used for
//  mathematics. 
//---------------------------------------------------------------------------//
using UnityEngine;

namespace Lemongrass
{
  /**
   * @brief A collection of utility functions that can be used for mathematics. 
   */
  public static class LemonVectors
  {
    
    /**
     * @brief Gets the point at which the given ray intersects with a plane described by 
     *        planePoint and planeNormal. 
     * @param ray The ray tested for intersection
     * @param planePoint An arbitrary point on the plane being intersected.
     * @param planeNormal The normal to the plane being intersected.
     * @return The point at which the ray intersects the plane. If there is no intersection
     *         a vector with max values is returned.
     */
    public static Vector3 GetPlaneRayIntersection(Ray ray, Vector3 planePoint, Vector3 planeNormal)
    {
      Vector3 O = ray.origin;
      Vector3 a = planePoint;
      Vector3 d = ray.direction;
      Vector3 N = planeNormal;

      float numerator = N.x * (O.x - a.x) + N.y * (O.y - a.y) + N.z * (O.z - a.z);
      float denominator = N.x * d.x + N.y * d.y + N.z * d.z;

      // Cannot divide by 0. No intersection
      if (denominator == 0) return new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

      float intersectT = numerator / -denominator;
      return ray.GetPoint(intersectT);
    }

    /**
     * @brief Gets the angle between two vectors as a number of degrees 0 to 360.
     *        Vector3.angle in contrast will provide an angle between -180 to 180 degrees
     * @param a The first vector
     * @param b The second vector
     * @return The angle between vector a and vector b.
     */
    public static float Angle360(Vector3 a, Vector3 b)
    {
      float angle = Vector3.Angle(a, b);
      float crossY = Vector3.Cross(a, b).y;
      // special cases that provide slightly nonzero crossproducts even though they should be zero
      if (angle == 0 || angle == 180) return angle;

      if (crossY < 0) return angle + 180;
      return angle;
    }
  }
}
