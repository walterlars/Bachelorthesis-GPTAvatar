// using Fusion;
// using UnityEngine;

// public class Player : NetworkBehaviour
// {
//     [Networked] public string playerName { get; set; }
//     [Networked] public bool isWalking { get; set; }
//     [Networked,  Capacity(100)] public string AvatarURL { get; set; }

//     public void SetAvatarURL(string url)
//     {
//         if (HasStateAuthority)
//         {
//             AvatarURL = url;
//             Debug.Log("Avatar URL set: " + AvatarURL);
//             Debug.Log("Avatar URL length: " + AvatarURL.Length);
//         }
//     }
// }

using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Networked] public string playerName { get; set; }
    [Networked] public bool isWalking { get; set; }
    [Networked, Capacity(100)] public NetworkString<_256> AvatarURL { get; set; }  // Ensure NetworkString capacity is set

    public void SetAvatarURL(string url)
    {
        if (HasStateAuthority)
        {
            AvatarURL = url;
            Debug.Log("Avatar URL set: " + AvatarURL);
        }
    }

    // Ensure network synchronization of AvatarURL
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            // Load avatar for the local player
            GetComponent<LoadAvatar>().LoadAvatarFromUrl(AvatarURL.ToString());
        }
        else
        {
            // Load avatar for other players
            GetComponent<LoadAvatar>().LoadAvatarFromUrl(AvatarURL.ToString());
        }
    }
}
