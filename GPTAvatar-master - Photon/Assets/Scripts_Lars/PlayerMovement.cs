// using System.Collections;
// using Fusion;
// using UnityEngine;
// using ReadyPlayerMe.Core;

// public class PlayerMovement : NetworkBehaviour
// {
//     private CharacterController _controller;
//     public Animator animator;
//     private Vector3 _velocity;
//     public Camera CameraPrefab; // Reference to the Camera prefab
//     private Camera _cameraInstance;

//     public float PlayerSpeed = 2f;
//     public float Gravity = -9.81f;
//     public float GroundCheckDistance = 0.1f;
//     public LayerMask GroundMask;
//     private bool _isGrounded;

//     private LoadAvatar loadAvatar;


//     private void Awake()
//     {
//         //  string playerNames = PlayerNameManager.GetPlayerNames();
//         // Debug.Log("Player Names: " + playerNames);
//         StartCoroutine(SearchAnimatorWithDelay());
//         _controller = GetComponent<CharacterController>();
//     }
    
//       private IEnumerator SearchAnimatorWithDelay()
//     {
//         // Wait for 1 second
//         Debug.Log("Waiting for 1 second...");
//         yield return new WaitForSeconds(2.0f);
//         Debug.Log("1 second has passed.");

//         // Proceed to search for the Animator component
//          // Step 1: Get the root GameObject (this script is attached to it)
//         GameObject root = this.gameObject;
//         loadAvatar = GetComponent<LoadAvatar>();
//         string avatarUrl = loadAvatar.GetAvatar();
//         Debug.LogError(avatarUrl);

//         //638df693d72bffc6fa17943c

//         // Step 2: Find the AvatarContainer child GameObject
//         Transform avatarContainerTransform = root.transform.Find("AvatarContainer");
//         if (avatarContainerTransform != null)
//         {
//             // Step 3: Find the 1bd72 child GameObject
//             Transform Avatar = avatarContainerTransform.Find(avatarUrl);
//             if (Avatar != null)
//             {
//                 // Step 4: Get the Animator component from the 1bd72 GameObject
//                 animator = Avatar.GetComponent<Animator>();
//                 if (animator != null)
//                 {
//                     Debug.Log("Animator found!");
//                 }
//                 else
//                 {
//                     Debug.LogError("Animator component not found on 1bd72 GameObject.");
//                 }
//             }
//             else
//             {
//                 Debug.LogError("1bd72 GameObject not found under AvatarContainer.");
//             }
//         }
//         else
//         {
//             Debug.LogError("AvatarContainer GameObject not found.");
//         }
//     }



      

//     public override void Spawned()
//     {
       
//             if (HasStateAuthority)
//             {
//                 // Instantiate and assign a camera for each player
//                 _cameraInstance = Instantiate(CameraPrefab);
//                 FirstPersonCamera firstPersonCamera = _cameraInstance.GetComponent<FirstPersonCamera>();
//                 firstPersonCamera.Target = transform;
//                 firstPersonCamera.CameraHeightOffset = 1.5f;
//             } 
   
//     }
    
//     public override void FixedUpdateNetwork()
//     {
//         // Only move own player and not every other player. Each player controls its own player object.
//         if (!HasStateAuthority)
//         {
//             return;
//         }

//         // Ground check
//         _isGrounded = Physics.CheckSphere(transform.position + Vector3.down * GroundCheckDistance, GroundCheckDistance, GroundMask);

//         if (_isGrounded && _velocity.y < 0)
//         {
//             _velocity.y = -2f; // Ensures the character stays grounded
//         }

//         // Get movement input
//         Quaternion cameraRotationY = Quaternion.Euler(0, _cameraInstance.transform.rotation.eulerAngles.y, 0);
//         Vector3 move = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * PlayerSpeed;

//         // Apply gravity
//         _velocity.y += Gravity * Runner.DeltaTime;

//         // Move the character
//         _controller.Move(move * Runner.DeltaTime);
//         _controller.Move(_velocity * Runner.DeltaTime);

//         // Update animation state
//         if (move != Vector3.zero)
//         {   
           
//             animator.SetBool("Walking", true);
//             transform.forward = move; // Rotate the character to face the movement direction
//         }
//         else
//         {
//             animator.SetBool("Walking", false);
//         }
//     }
    
// }


using System.Collections;
using Fusion;
using UnityEngine;
using ReadyPlayerMe.Core;

public class PlayerMovement : NetworkBehaviour
{
    private CharacterController _controller;
    public Animator animator;
    private Vector3 _velocity;
    public Camera CameraPrefab; // Reference to the Camera prefab
    private Camera _cameraInstance;
    public float PlayerSpeed = 2f;
    public float Gravity = -9.81f;
    public float GroundCheckDistance = 0.1f;
    public LayerMask GroundMask;
    private bool _isGrounded;

    private LoadAvatar loadAvatar;

    private void Awake()
    {
        StartCoroutine(SearchAnimatorWithDelay());
        _controller = GetComponent<CharacterController>();
    }

    private IEnumerator SearchAnimatorWithDelay()
    {
        Debug.Log("Waiting for 2 seconds...");
        yield return new WaitForSeconds(2.0f);
        Debug.Log("2 seconds have passed.");

        GameObject root = this.gameObject;
        loadAvatar = GetComponent<LoadAvatar>();
        string avatarUrl = loadAvatar.GetAvatar();
        Debug.LogError(avatarUrl);

        Transform avatarContainerTransform = root.transform.Find("AvatarContainer");
        if (avatarContainerTransform != null)
        {
            Transform Avatar = avatarContainerTransform.Find(avatarUrl);
            if (Avatar != null)
            {
                animator = Avatar.GetComponent<Animator>();
                if (animator != null)
                {
                    Debug.Log("Animator found!");
                }
                else
                {
                    Debug.LogError("Animator component not found on Avatar GameObject.");
                }
            }
            else
            {
                Debug.LogError("Avatar GameObject not found under AvatarContainer.");
            }
        }
        else
        {
            Debug.LogError("AvatarContainer GameObject not found.");
        }
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            _cameraInstance = Instantiate(CameraPrefab);
            FirstPersonCamera firstPersonCamera = _cameraInstance.GetComponent<FirstPersonCamera>();
            firstPersonCamera.Target = transform;
            firstPersonCamera.CameraHeightOffset = 1.5f;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        _isGrounded = Physics.CheckSphere(transform.position + Vector3.down * GroundCheckDistance, GroundCheckDistance, GroundMask);

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // Ensures the character stays grounded
        }

        Quaternion cameraRotationY = Quaternion.Euler(0, _cameraInstance.transform.rotation.eulerAngles.y, 0);
        Vector3 move = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * PlayerSpeed;

        _velocity.y += Gravity * Runner.DeltaTime;

        _controller.Move(move * Runner.DeltaTime);
        _controller.Move(_velocity * Runner.DeltaTime);

        bool isWalking = move != Vector3.zero;
        if (isWalking != animator.GetBool("Walking"))
        {
            RPC_UpdateWalkingState(isWalking);
        }

        if (isWalking)
        {
            transform.forward = move; // Rotate the character to face the movement direction
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_UpdateWalkingState(bool walking)
    {
        animator.SetBool("Walking", walking);
    }
}
