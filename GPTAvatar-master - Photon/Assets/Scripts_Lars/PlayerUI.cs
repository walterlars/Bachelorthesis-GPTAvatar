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
        StartCoroutine(FindLocalPlayerCoroutine());
    }

    private IEnumerator FindLocalPlayerCoroutine()
    {
        while (localPlayer == null)
        {
            localPlayer = FindLocalPlayer();
            yield return new WaitForSeconds(0.5f);
        }

        if (localPlayer != null)
        {
            playerNameText.text = localPlayer.playerName;
            AvatarURLText.text = localPlayer.AvatarURL.Value;
        }
    }

    private Player FindLocalPlayer()
    {
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
