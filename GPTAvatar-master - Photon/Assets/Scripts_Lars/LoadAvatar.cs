using ReadyPlayerMe.Core;
using UnityEngine;
using Fusion;
using System.Collections;

public class LoadAvatar : NetworkBehaviour
{
    [SerializeField] [Tooltip("Set this to the container where the avatar will be instantiated.")]
    private Transform avatarContainer;

    [SerializeField] [Tooltip("Animator controller to be assigned to the loaded avatar.")]
    private RuntimeAnimatorController animatorController;

    private GameObject avatar;
    private Animator avatarAnimator;

    private string avatarURL;
    private bool isAvatarLoading = false;
    private bool isAvatarLoaded = false;

    private bool IsValidAvatarUrl(string url)
    {
        return !string.IsNullOrEmpty(url) &&
               url.StartsWith("https://models.readyplayer.me") &&
               url.EndsWith(".glb");
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Player player = GetComponent<Player>();
            avatarURL = player.AvatarURL.ToString();
            if (!isAvatarLoading && !isAvatarLoaded)
            {
                StartCoroutine(LoadAvatarCoroutine(avatarURL));
            }
        }
    }

    public IEnumerator LoadAvatarCoroutine(string url)
    {
        isAvatarLoading = true;
        yield return new WaitUntil(() => IsValidAvatarUrl(url));
        LoadAvatarFromUrl(url);
    }

    public void LoadAvatarFromUrl(string url)
    {
        if (isAvatarLoaded || isAvatarLoading) return;

        isAvatarLoading = true;

        var avatarLoader = new AvatarObjectLoader();
        avatarLoader.OnCompleted += (_, args) =>
        {
            if (isAvatarLoaded) return;

            avatar = args.Avatar;
            avatarAnimator = avatar.GetComponent<Animator>();
            avatar.AddComponent<NetworkObject>();
          

            avatar = args.Avatar;

            if (avatarAnimator != null && animatorController != null)
            {
                avatarAnimator.runtimeAnimatorController = animatorController;
                avatar.AddComponent<NetworkAnimatorSetup>();
                Debug.Log("Animator controller assigned to avatar's Animator.");
            }
            else if (avatarAnimator == null)
            {
                Debug.LogError("Animator component is missing on the loaded avatar.");
            }
            else
            {
                Debug.LogError("AnimatorController is missing.");
            }

            args.Avatar.transform.SetParent(avatarContainer, false);
            args.Avatar.transform.localPosition = Vector3.zero;
            args.Avatar.transform.localRotation = Quaternion.identity;

            isAvatarLoaded = true;
            isAvatarLoading = false;
            Debug.Log("Avatar loaded and set up successfully.");
        };
        avatarLoader.LoadAvatar(url);
    }

    private void OnDestroy()
    {
        if (avatar != null) Destroy(avatar);
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

    public string GetAvatar()
    {
        Debug.Log("Avatar URL: " + avatarURL);
        return ExtractPartFromURL(avatarURL);
    }
}


