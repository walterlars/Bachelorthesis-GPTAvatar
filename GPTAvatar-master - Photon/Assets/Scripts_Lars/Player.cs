using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Networked] public string playerName { get; set; }
    [Networked] public bool isWalking { get; set; }
    [Networked, Capacity(100)] public NetworkString<_256> AvatarURL { get; set; }  

    public void SetAvatarURL(string url)
    {
        if (HasStateAuthority)
        {
            AvatarURL = url;
            Debug.Log("Avatar URL set: " + AvatarURL);
        }
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            GetComponent<LoadAvatar>().LoadAvatarFromUrl(AvatarURL.ToString());
        }
        else
        {
            GetComponent<LoadAvatar>().LoadAvatarFromUrl(AvatarURL.ToString());
        }
    }
}
