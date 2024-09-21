using UnityEngine;
using System.Collections;
using Fusion;
using TMPro;

public class SitOnChair : NetworkBehaviour
{
    public float interactionDistance = 2.0f; 
    public Animator animator;
    private CharacterController _characterController;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private LoadAvatar loadAvatar;
    private bool animatorReady = false;

    [Networked]
    public NetworkBool IsSitting { get; set; }
    private TextMeshProUGUI avatarURLText;

    private void Awake()
    {
        StartCoroutine(SearchAnimatorWithDelay());
        _characterController = GetComponent<CharacterController>();
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

        public string ExtractPartFromURL(string url)
    {
        string prefix = "https://models.readyplayer.me/";
        string suffix = ".glb";

        int startIndex = url.IndexOf(prefix) + prefix.Length;
        int endIndex = url.IndexOf(suffix);
        string extractedPart = url.Substring(startIndex, endIndex - startIndex);

        return extractedPart;
    }

    private IEnumerator SearchAnimatorWithDelay()
    {
        Debug.Log("Waiting for 2 seconds...");
        yield return new WaitForSeconds(2.0f);
        Debug.Log("2 seconds have passed.");

        GameObject root = this.gameObject;
        Player player = GetComponent<Player>();
        string avatarURLlong = player.AvatarURL.ToString();
        string avatarUrl = ExtractPartFromURL(avatarURLlong);
        
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
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;

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
                        RPC_UpdateSittingState(true);
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

        yield return new WaitForSeconds(0.5f);

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


    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_UpdateSittingState(bool sitting)
    {
        if (!animatorReady)
        {
            Debug.LogError("Animator is not ready");
            return;
        }

        animator.SetBool("isSitting", sitting);
    }

}

