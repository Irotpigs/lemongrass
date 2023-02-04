//---------------------------------------------------------------------------//
// Name        - LemonFirstPersonCamera.cs
// Author      - Daniel
// Project     - Lemongrass
// Description - Uses the Unity InputSystem to convert action bindings into 
//  simple camera movement. This camera is intended to be parented to the
//  player mesh playerBodyMesh at the eye level
//---------------------------------------------------------------------------//
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lemongrass
{
  /**
   * @brief Uses the Unity InputSystem to convert action bindings into 
   *        simple camera movement.
   */
  [RequireComponent(typeof(PlayerInput))]
  public class LemonFirstPersonCamera : MonoBehaviour
  {
    ////////////////////////////////////Variables//////////////////////////////////
    //-private-------------------------------------------------------------------//
    [SerializeField] private float mouseSensitivity = 1.6f;

    [SerializeField, Tooltip("This is the mesh that should be rotated to face with the camera.\nCan be null.")]
    private Transform playerBodyMesh;

    float xRotation = 0f;

    ////////////////////////////////////Functions//////////////////////////////////
    //-private-------------------------------------------------------------------//
    // Start is called before the first frame update
    void Start()
    {
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
    }

    ////////////////////////////////Player Input Messages///////////////////////////////
    private void OnLook(InputValue value)
    {
      //Get Mouse Input
      float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
      float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

      xRotation -= mouseY;
      xRotation = Mathf.Clamp(xRotation, -90f, 90f);

      transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
      playerBodyMesh.Rotate(Vector3.up * mouseX);
    }
    ////////////////////////////////////////////////////////////////////////////////////
  }
}
