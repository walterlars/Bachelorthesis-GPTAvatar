// using ReadyPlayerMe.Core;
// using UnityEngine;
// using Fusion;
// using System.Collections;

// public class LoadAvatar : NetworkBehaviour
// {
//     [SerializeField] [Tooltip("Set this to the URL or shortcode of the Ready Player Me Avatar you want to load.")]
//     private string avatarUrl = "https://models.readyplayer.me/638df693d72bffc6fa17943c.glb";

//     [SerializeField] [Tooltip("Set this to the container where the avatar will be instantiated.")]
//     private Transform avatarContainer;

//     [SerializeField] [Tooltip("Animator controller to be assigned to the loaded avatar.")]
//     private RuntimeAnimatorController animatorController;

//     private GameObject avatar;
//     private Animator avatarAnimator; 
//     private Player localPlayer;

//     private bool IsValidAvatarUrl(string url)
//     {
//         return !string.IsNullOrEmpty(url) &&
//                url.StartsWith("https://models.readyplayer.me") &&
//                url.EndsWith(".glb");
//     }

//     private void Start()
//     {
//         // Start coroutine to find the local player
//         StartCoroutine(FindLocalPlayerCoroutine());
       

//     }

//     private IEnumerator FindLocalPlayerCoroutine()
//     {
//         while (localPlayer == null)
//         {
//             localPlayer = FindLocalPlayer();
//             yield return new WaitForSeconds(0.5f);
//         }

//         // Proceed to load avatar once the local player is found
//         Debug.Log("Retrieved Avatar URL: " + localPlayer.AvatarURL); // Debug the URL
//         Debug.Log("Retrieved Avatar URL length: " + localPlayer.AvatarURL.Length); // Debug the URL length

//         if (IsValidAvatarUrl(localPlayer.AvatarURL))
//         {
//             avatarUrl = localPlayer.AvatarURL;
//             LoadAvatarFromUrl(avatarUrl);
//             Debug.Log("Avatar URL: " + localPlayer.AvatarURL);
//         }
//         else
//         {
//             LoadAvatarFromUrl(avatarUrl);
//             Debug.LogError("The URL is not valid.");
//             Debug.Log("Avatar URL: " + localPlayer.AvatarURL);
//         }
//     }

//     public void printAvatar(){
//         localPlayer = FindLocalPlayer();
//         Debug.Log("Avatar URL: " + localPlayer.AvatarURL);
//     }

//     private Player FindLocalPlayer()
//     {
//         // Find the local player object in the scene
//         foreach (var networkObject in FindObjectsOfType<NetworkObject>())
//         {
//             if (networkObject.HasInputAuthority)
//             {
//                 return networkObject.GetComponent<Player>();
//             }
//         }
//         return null;
//     }

//     private void LoadAvatarFromUrl(string url)
//     {
//         var avatarLoader = new AvatarObjectLoader();
//         // use the OnCompleted event to set the avatar and setup animator
//         avatarLoader.OnCompleted += (_, args) =>
//         {
//             avatar = args.Avatar;
//             Debug.Log("Avatar loaded successfully.");

//             // Get the Animator component from the loaded avatar
//             avatarAnimator = avatar.GetComponent<Animator>();
//             NetworkAnimatorSetup networkAnimatorSetup = avatar.AddComponent<NetworkAnimatorSetup>();
//             NetworkObject networkObject = avatar.AddComponent<NetworkObject>();

//             // Assign the provided animator controller to the avatar's Animator
//             if (avatarAnimator != null && animatorController != null)
//             {
//                 avatarAnimator.runtimeAnimatorController = animatorController;
//                 Debug.Log("Animator controller assigned to avatar's Animator.");
//             }
//             else if (avatarAnimator == null)
//             {
//                 Debug.LogError("Animator component is missing on the loaded avatar.");
//             }
//             else
//             {
//                 Debug.LogError("AnimatorController is missing.");
//             }

//             // Set the avatar as a child of the AvatarContainer
//             args.Avatar.transform.SetParent(avatarContainer, false);
//             args.Avatar.transform.localPosition = Vector3.zero;
//             args.Avatar.transform.localRotation = Quaternion.identity;
//         };
//         avatarLoader.LoadAvatar(url);
//     }

//     private void OnDestroy()
//     {
//         if (avatar != null) Destroy(avatar);
//     }

//     public string GetAvatar()
//     {
//         // if (localPlayer != null && IsValidAvatarUrl(localPlayer.AvatarURL))
//         // {
//         //     return ExtractPartFromURL(avatarUrl);
//         // }
//         return ExtractPartFromURL(avatarUrl);
//     }

//     string ExtractPartFromURL(string url)
//     {
//         // Define the prefix and suffix to search for
//         string prefix = "https://models.readyplayer.me/";
//         string suffix = ".glb";

//         // Find the starting index of the part to extract
//         int startIndex = url.IndexOf(prefix) + prefix.Length;
//         // Find the ending index of the part to extract
//         int endIndex = url.IndexOf(suffix);

//         // Extract the part between the prefix and suffix
//         string extractedPart = url.Substring(startIndex, endIndex - startIndex);

//         return extractedPart;
//     }
// }

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

            if (avatarAnimator != null && animatorController != null)
            {
                avatarAnimator.runtimeAnimatorController = animatorController;
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
        // Define the prefix and suffix to search for
        string prefix = "https://models.readyplayer.me/";
        string suffix = ".glb";

        // Find the starting index of the part to extract
        int startIndex = url.IndexOf(prefix) + prefix.Length;
        // Find the ending index of the part to extract
        int endIndex = url.IndexOf(suffix);

        // Extract the part between the prefix and suffix
        string extractedPart = url.Substring(startIndex, endIndex - startIndex);

        return extractedPart;
    }

    public string GetAvatar()
    {
        Debug.Log("Avatar URL: " + avatarURL);
        return ExtractPartFromURL(avatarURL);
    }
}


