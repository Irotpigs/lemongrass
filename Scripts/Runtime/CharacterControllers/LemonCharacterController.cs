//---------------------------------------------------------------------------//
// Name        - LemonCharacterController.cs
// Author      - Daniel
// Project     - Lemongrass
// Description - Uses the Unity InputSystem to convert action bindings into 
//  simple character movement. This class would have to be expanded to fit a 
//  given project as there isn't a one size fits all situation
//---------------------------------------------------------------------------//
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lemongrass
{
  /**
   * @brief A character controller that will listen for generic FPS Unity messages.
   *        Expected to be overhauled to match a given project.
   */
   [RequireComponent(typeof(PlayerInput))]
   [RequireComponent(typeof(Rigidbody))]
  public class LemonCharacterController : MonoBehaviour
  {
    ////////////////////////////////////Variables//////////////////////////////////
    //-private-------------------------------------------------------------------//
    [Header("Setup")]
    [SerializeField] private Transform playerModelTransform;
    [SerializeField] private float playerModelHeight = 1.0f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement stats")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float accelerationMultiplier = 8f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float airMultiplier = 1.2f;
    [SerializeField] private float gravityMultiplier = 1f;
    [SerializeField] private float groundDrag = 10f;

    private Rigidbody playerRb;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;

    private bool grounded = true;

    ////////////////////////////////////Functions//////////////////////////////////
    //-private-------------------------------------------------------------------//
    private void Awake()
    {
      playerRb = GetComponent<Rigidbody>();
      playerRb.freezeRotation = true;
      playerRb.useGravity = false;
    }

    private void Update()
    {
      // NOTE: grounded uses a raycast to determine grounded state. Ideally this should be removed
      grounded = Physics.Raycast(playerModelTransform.position, Vector3.down, playerModelHeight * 0.56f, groundLayer);

      // update drag state
      if (grounded) playerRb.drag = groundDrag;
      else playerRb.drag = 0;
    }

    private void FixedUpdate()
    {
      MovePlayer();
      Fall();
    }

    private void MovePlayer()
    {
      // calculate movement direction
      moveDirection = playerModelTransform.forward * verticalInput + playerModelTransform.right * horizontalInput;

      // NOTE: this is not controlled by any deltaTime 
      if (grounded) playerRb.AddForce(moveDirection.normalized * moveSpeed * accelerationMultiplier, ForceMode.Force);
      else if (!grounded) playerRb.AddForce(moveDirection.normalized * moveSpeed * accelerationMultiplier * airMultiplier, ForceMode.Force);

      // clamp the movement speed on a horizontal plane
      Vector3 flatVel = new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z);
      if (flatVel.sqrMagnitude > (moveSpeed * moveSpeed))
      {
        Vector3 limitedVel = flatVel.normalized * moveSpeed;
        playerRb.velocity = new Vector3(limitedVel.x, playerRb.velocity.y, limitedVel.z);
      }
    }

    private void Fall()
    {
      // move the player as though it had gravity
      float accelerationDueToGravity = playerRb.velocity.y + Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
      playerRb.velocity = new Vector3(playerRb.velocity.x, accelerationDueToGravity, playerRb.velocity.z);
    }

    ////////////////////////////////Player Input Messages///////////////////////////////
    private void OnMove(InputValue value)
    {
      horizontalInput = value.Get<Vector2>().x;
      verticalInput = value.Get<Vector2>().y;
    }

    private void OnJump(InputValue value)
    {
      // NOTE: room here for double jumping, etc.
      if (!grounded) return;

      // reset y velocity
      playerRb.velocity = new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z);

      playerRb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    ////////////////////////////////////////////////////////////////////////////////////
  }
}
