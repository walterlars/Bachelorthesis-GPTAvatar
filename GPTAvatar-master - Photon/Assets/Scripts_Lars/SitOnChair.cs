// using UnityEngine;
// using System.Collections;
// using Fusion;

// public class SitOnChair : NetworkBehaviour
// {
//     public float interactionDistance = 2.0f; // The distance within which the character can interact with the chair
//     public Animator animator;
//     private CharacterController _characterController;
//     private Vector3 _initialPosition; // To store the initial standing position
//     private Quaternion _initialRotation; // To store the initial standing rotation

//     private LoadAvatar loadAvatar;

//     [Networked]
//     public NetworkBool IsSitting { get; set; }

//     private void Awake()
//     {
//         StartCoroutine(SearchAnimatorWithDelay());
//         // _animator = GetComponent<Animator>(); 
//         _characterController = GetComponent<CharacterController>(); // Ensure the character has a CharacterController component
//         _initialPosition = transform.position; // Store the initial position
//         _initialRotation = transform.rotation; // Store the initial rotation
//     }

//      private IEnumerator SearchAnimatorWithDelay()
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
//             // Step 3: Find the child GameObject
//             Transform Avatar = avatarContainerTransform.Find(avatarUrl);
//             if (Avatar != null)
//             {
//                 // Step 4: Get the Animator component from the GameObject
//                 animator = Avatar.GetComponent<Animator>();
//                 if (animator != null)
//                 {
//                     Debug.Log("Animator found!");
//                 }
//                 else
//                 {
//                     Debug.LogError("Animator component not found on GameObject.");
//                 }
//             }
//             else
//             {
//                 Debug.LogError("GameObject not found under AvatarContainer.");
//             }
//         }
//         else
//         {
//             Debug.LogError("AvatarContainer GameObject not found.");
//         }
//     }


//     private void Update()
//     {
//         if (HasStateAuthority && Input.GetKeyDown(KeyCode.E))
//         {
//             _initialPosition = transform.position; // Store the initial position
//             _initialRotation = transform.rotation; // Store the initial rotation

//             if (IsSitting)
//             {
//                 RPC_StandUp();
//             }
//             else
//             {
//                 TrySitOnChair();
//             }
//         }
//     }

//     private void TrySitOnChair()
//     {
//         if (IsSitting) return;

//         Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionDistance);
//         foreach (var hitCollider in hitColliders)
//         {
//             if (hitCollider.CompareTag("Chair"))
//             {
//                 Chair chair = hitCollider.GetComponent<Chair>();
//                 if (chair != null && !chair.IsOccupied)
//                 {
//                     Transform potentialChairPosition = hitCollider.transform.Find("ChairPosition");
//                     if (potentialChairPosition != null)
//                     {
//                         RPC_Sit(potentialChairPosition.position, potentialChairPosition.rotation, chair.GetComponent<NetworkObject>());
//                         break;
//                     }
//                 }
//             }
//         }
//     }

//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     private void RPC_Sit(Vector3 chairPosition, Quaternion chairRotation, NetworkObject chairObject)
//     {
//         IsSitting = true;

//         // Log the chair position for debugging
//         Debug.Log("Sitting at chair position: " + chairPosition);

//         // Play sitting animation
//         // animator.SetBool("isSitting", true);
//         RPC_UpdateSittinggState(true);

//         // Move the character to the chair position and rotation after the animation
//         StartCoroutine(MoveToChairPosition(chairPosition, chairRotation, chairObject));
//     }

//     private IEnumerator MoveToChairPosition(Vector3 chairPosition, Quaternion chairRotation, NetworkObject chairObject)
//     {
//         // Optionally disable character movement and other components
//         if (_characterController != null)
//         {
//             _characterController.enabled = false;
//         }

//         // Wait for a short duration to simulate animation delay
//         yield return new WaitForSeconds(0.5f); // Adjust this duration to match your animation length

//         // Move the character to the chair position and rotation
//         transform.position = chairPosition;
//         transform.rotation = chairRotation;

//         Debug.Log("Moved to chair position: " + chairPosition);

//         // Mark the chair as occupied
//         Chair chair = chairObject.GetComponent<Chair>();
//         if (chair != null)
//         {
//             chair.IsOccupied = true;
//         }
//     }

//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     private void RPC_StandUp()
//     {
//         IsSitting = false;

//         // Log standing up for debugging
//         Debug.Log("Standing up");

//         // Play standing up animation
//         // animator.SetBool("isSitting", false);
//         RPC_UpdateSittinggState(false);

//         // Restore the initial standing position and rotation
//         transform.position = _initialPosition;
//         transform.rotation = _initialRotation;

//         // Enable CharacterController after standing up
//         if (_characterController != null)
//         {
//             _characterController.enabled = true;
//         }

//         // Mark the chair as unoccupied
//         Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionDistance);
//         foreach (var hitCollider in hitColliders)
//         {
//             if (hitCollider.CompareTag("Chair"))
//             {
//                 Chair chair = hitCollider.GetComponent<Chair>();
//                 if (chair != null && chair.IsOccupied)
//                 {
//                     chair.IsOccupied = false;
//                     break;
//                 }
//             }
//         }
//     }

//     [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
//     private void RPC_UpdateSittinggState(bool sitting)
//     {
//         animator.SetBool("isSitting", sitting);
//     }
// }

using UnityEngine;
using System.Collections;
using Fusion;

public class SitOnChair : NetworkBehaviour
{
    public float interactionDistance = 2.0f; // The distance within which the character can interact with the chair
    public Animator animator; // This can be assigned in the Inspector
    private CharacterController _characterController;
    private Vector3 _initialPosition; // To store the initial standing position
    private Quaternion _initialRotation; // To store the initial standing rotation
    private LoadAvatar loadAvatar;
    private bool animatorReady = false;

    [Networked]
    public NetworkBool IsSitting { get; set; }

    private void Awake()
    {
        StartCoroutine(SearchAnimatorWithDelay());
        _characterController = GetComponent<CharacterController>(); // Ensure the character has a CharacterController component
        _initialPosition = transform.position; // Store the initial position
        _initialRotation = transform.rotation; // Store the initial rotation
    }

    private IEnumerator SearchAnimatorWithDelay()
    {
        // Wait for 2 seconds
        Debug.Log("Waiting for 2 seconds...");
        yield return new WaitForSeconds(2.0f);
        Debug.Log("2 seconds have passed.");

        // Proceed to search for the Animator component
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
                    animatorReady = true;
                }
                else
                {
                    Debug.LogError("Animator component not found on GameObject.");
                }
            }
            else
            {
                Debug.LogError("GameObject not found under AvatarContainer.");
            }
        }
        else
        {
            Debug.LogError("AvatarContainer GameObject not found.");
        }
    }

    private void Update()
    {
        if (!animatorReady) return;

        if (HasStateAuthority && Input.GetKeyDown(KeyCode.E))
        {
            _initialPosition = transform.position; // Store the initial position
            _initialRotation = transform.rotation; // Store the initial rotation

            if (IsSitting)
            {
                RPC_StandUp();
            }
            else
            {
                TrySitOnChair();
            }
        }
    }

    private void TrySitOnChair()
    {
        if (IsSitting || !animatorReady) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionDistance);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Chair"))
            {
                Chair chair = hitCollider.GetComponent<Chair>();
                if (chair != null && !chair.IsOccupied)
                {
                    Transform potentialChairPosition = hitCollider.transform.Find("ChairPosition");
                    if (potentialChairPosition != null)
                    {
                        RPC_Sit(potentialChairPosition.position, potentialChairPosition.rotation, chair.GetComponent<NetworkObject>());
                        break;
                    }
                }
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_Sit(Vector3 chairPosition, Quaternion chairRotation, NetworkObject chairObject)
    {
        if (!animatorReady) return;

        IsSitting = true;
        RPC_UpdateSittingState(true);
        StartCoroutine(MoveToChairPosition(chairPosition, chairRotation, chairObject));
    }

    private IEnumerator MoveToChairPosition(Vector3 chairPosition, Quaternion chairRotation, NetworkObject chairObject)
    {
        if (_characterController != null)
        {
            _characterController.enabled = false;
        }

        yield return new WaitForSeconds(0.5f); // Adjust this duration to match your animation length

        transform.position = chairPosition;
        transform.rotation = chairRotation;

        Chair chair = chairObject.GetComponent<Chair>();
        if (chair != null)
        {
            chair.IsOccupied = true;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_StandUp()
    {
        if (!animatorReady) return;

        IsSitting = false;
        RPC_UpdateSittingState(false);
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;

        if (_characterController != null)
        {
            _characterController.enabled = true;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionDistance);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Chair"))
            {
                Chair chair = hitCollider.GetComponent<Chair>();
                if (chair != null && chair.IsOccupied)
                {
                    chair.IsOccupied = false;
                    break;
                }
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateSittingState(bool sitting)
    {
        if (!animatorReady)
        {
            Debug.LogError("Animator is not ready");
            return;
        }

        if (!HasStateAuthority)
        {
            Debug.LogError("Cannot send this RPC without State Authority");
            return;
        }

        animator.SetBool("isSitting", sitting);
    }
}

