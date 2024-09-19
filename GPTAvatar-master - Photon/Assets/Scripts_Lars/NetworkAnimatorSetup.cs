// using UnityEngine;
// using Fusion;

// public class NetworkAnimatorSetup : NetworkBehaviour
// {
//     private Animator animator;
//     private NetworkMecanimAnimator networkMecanimAnimator;

//     void Awake()
//     {
//         animator = GetComponent<Animator>();
//         if (animator == null)
//         {
//             Debug.LogError("Animator component not found on the GameObject.");
//             return;
//         }

//         networkMecanimAnimator = gameObject.AddComponent<NetworkMecanimAnimator>();
//         networkMecanimAnimator.Animator = animator;

//         // Enable the NetworkMecanimAnimator component
//         networkMecanimAnimator.enabled = true;
//     }

//     public override void Spawned()
//     {
//         // Ensure the NetworkMecanimAnimator is enabled after the object is spawned
//         if (!networkMecanimAnimator.enabled)
//         {
//             networkMecanimAnimator.enabled = true;
//         }
//     }

//     void Start()
//     {
//         // Ensure the NetworkMecanimAnimator is enabled at the start
//         if (!networkMecanimAnimator.enabled)
//         {
//             networkMecanimAnimator.enabled = true;
//         }
//     }
// }


using UnityEngine;
using Fusion;

public class NetworkAnimatorSetup : NetworkBehaviour
{
    private Animator animator;
    private NetworkMecanimAnimator networkMecanimAnimator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the GameObject.");
            return;
        }

        networkMecanimAnimator = gameObject.AddComponent<NetworkMecanimAnimator>();
        networkMecanimAnimator.Animator = animator;

        networkMecanimAnimator.enabled = true;
    }

    public override void Spawned()
    {
        if (!networkMecanimAnimator.enabled)
        {
            networkMecanimAnimator.enabled = true;
        }
    }

    void Start()
    {
        if (!networkMecanimAnimator.enabled)
        {
            networkMecanimAnimator.enabled = true;
        }
    }
}
