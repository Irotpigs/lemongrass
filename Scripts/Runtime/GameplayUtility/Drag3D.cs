//---------------------------------------------------------------------------//
// Name        - Drag3D.cs
// Author      - Daniel Onstott
// Project     - Lemongrass
// Description - A utility component for objects that need to be dragged in a
//  simple way in a 3D environment. The objects will be constrained to a plane
//  defined by the vector that they are lifted on.
//---------------------------------------------------------------------------//
using UnityEngine;

namespace Lemongrass
{
  /**
   * @brief A utility component for dragging 3D objects. The object is moved on
   *  the plane defined by liftVector.
   */
  public class Drag3D : MonoBehaviour
  {
    ////////////////////////////////////Variables//////////////////////////////////
    //-public--------------------------------------------------------------------//

    // The vector that the object is lifted on. Will be normalized upon setting
    public Vector3 LiftVector
    {
      get { return liftVector; }
      set { liftVector = value.normalized; }
    }

    //-private-------------------------------------------------------------------//
    [Header("Movement Variables")]
    [SerializeField, Tooltip("Controls the speed at which the dragged object lags behind")] 
    private float dampingSpeed = 15;
    [SerializeField, Tooltip("Height in Unity units that the object will move up the lift vector when dragged.")]
    private float liftHeight = 0.5f;
    [SerializeField, Tooltip("The vector that the object is lifted on. Will be normalized")]
    private Vector3 liftVector = -Physics.gravity.normalized;

    [Header("Options")]
    [SerializeField, Tooltip("Should this drag move the root object's transform.")]
    private bool movesRootTransform = true;

    /** @brief determines the transform that will be moved on drag */
    private Transform objectBaseTransform = null;

    /** @brief Tracks if the object is currently being dragged */
    private bool isDragging = false;

    /** @brief Tracks the point at which the object was picked up */
    private Vector3 dragStartPosition;

    ////////////////////////////////////Functions//////////////////////////////////
    //-drag control interface----------------------------------------------------//

    /**
     * @brief Called when the object starts being dragged
     */
    protected virtual void OnDragStarted()
    {
      // The needs for this are going to vary between projects.
      // Doesn't need to do anything for basic functionality
    }

    /**
     * @brief Called when the object ceases being dragged.
     */
    protected virtual void OnDragEnded()
    {
      // The needs for this are going to vary between projects.

      // This will drop the object immediately where it has been left
      objectBaseTransform.position = GetMousePlaneIntersection();
    }

    //-private-------------------------------------------------------------------//
    private void Awake()
    {
      // normalizes the lift vector so the lift height is accurate.
      liftVector.Normalize();

      if (movesRootTransform) objectBaseTransform = transform.root;
      else objectBaseTransform = transform;
    }

    private void OnMouseDown()
    {
      // This shouldn't ever be the case. Ensure that this object only gets dragged once at a time
      if (isDragging) return;
      isDragging = true;

      dragStartPosition = objectBaseTransform.position;
      OnDragStarted();
    }

    private void OnMouseUp()
    {
      isDragging = false;

      OnDragEnded();
    }

    private void Update()
    {
      if (isDragging)
      {
        Vector3 movementPlanePoint = GetMousePlaneIntersection();

        // Smoothly move the object towards the mouse
        objectBaseTransform.position = Vector3.Lerp(transform.position, movementPlanePoint + LiftVector * liftHeight, dampingSpeed * Time.deltaTime);
      }
    }

    /**
     * @brief Gets a point on the dragged object's plane intersecting with the mouse ray.
     * @return A vector3 on the object's plane of movement.
     */
    private Vector3 GetMousePlaneIntersection()
    {
      Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
      return LemonVectors.GetPlaneRayIntersection(mouseRay, dragStartPosition, LiftVector);
    }
  }
}
