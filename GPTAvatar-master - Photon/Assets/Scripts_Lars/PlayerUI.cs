using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI AvatarURLText;
    private Player localPlayer;

    private void Start()
    {
        // Wait for the player to be spawned
        StartCoroutine(FindLocalPlayerCoroutine());
    }

    private IEnumerator FindLocalPlayerCoroutine()
    {
        while (localPlayer == null)
        {
            localPlayer = FindLocalPlayer();
            yield return new WaitForSeconds(0.5f);
        }

        // Update the UI Text with the player's name when the local player is found
        if (localPlayer != null)
        {
            playerNameText.text = localPlayer.playerName;
            AvatarURLText.text = localPlayer.AvatarURL.Value;
        }
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
