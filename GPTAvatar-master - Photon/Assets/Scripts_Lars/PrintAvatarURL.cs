using ReadyPlayerMe.Core;
using UnityEngine;
using Fusion;
using System.Collections;

public class PrintAvatarURL : NetworkBehaviour
{
    private Player localPlayer;

    private void Start()
    {
        // Start coroutine to find the local player
        StartCoroutine(FindLocalPlayerCoroutine());
       

    }

    private IEnumerator FindLocalPlayerCoroutine()
    {
        while (localPlayer == null)
        {
            localPlayer = FindLocalPlayer();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void printAvatar(){
        localPlayer = FindLocalPlayer();
        Debug.Log("Avatar URL: " + localPlayer.AvatarURL);
    }

    private Player FindLocalPlayer()
    {
        // Find the local player object in the scene
        foreach (var networkObject in FindObjectsOfType<NetworkObject>())
        {
            if (networkObject.HasInputAuthority)
            {
                return networkObject.GetComponent<Player>();
            }
        }
        return null;
    }

    
}
