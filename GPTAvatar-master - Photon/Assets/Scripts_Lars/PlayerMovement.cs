using System.Collections;
using Fusion;
using UnityEngine;
using ReadyPlayerMe.Core;
using TMPro;

public class PlayerMovement : NetworkBehaviour
{
    private CharacterController _controller;
    public Animator animator;
    private Vector3 _velocity;
    public Camera CameraPrefab; 
    private Camera _cameraInstance;
    public float PlayerSpeed = 2f;
    public float Gravity = -9.81f;
    public float GroundCheckDistance = 0.1f;
    public LayerMask GroundMask;
    private bool _isGrounded;

    private LoadAvatar loadAvatar;

    private TextMeshProUGUI avatarURLText;



    private void Awake()
    {
        StartCoroutine(SearchAnimatorWithDelay());
        _controller = GetComponent<CharacterController>();
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
            _velocity.y = -2f;
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
            transform.forward = move;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_UpdateWalkingState(bool walking)
    {
        animator.SetBool("Walking", walking);
    }
}
