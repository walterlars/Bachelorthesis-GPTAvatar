// using Fusion;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Collections.Generic;


// public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
// {
//     public GameObject PlayerPrefab;

//     public TMP_Text playerCountText; // Link this in the Inspector
    

//     private int playerCount = 0;


//     public void PlayerJoined(PlayerRef player)
//     {

//         if (player == Runner.LocalPlayer)
//         {
//             var fusionBootstrap = FindObjectOfType<FusionBootstrap>();
//             string uniqueName = fusionBootstrap.PlayerName;
//             string avatarURL = fusionBootstrap.AvatarURL;

//         //   string uniqueName = GenerateUniquePlayerName();
//         //   Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity) =>
//             Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player, (runner, spawnedPlayer) =>
//             {
//                 Player playerScript = spawnedPlayer.GetComponent<Player>();
//                 playerScript.playerName = uniqueName;
//                 playerScript.AvatarURL = avatarURL;

//             });
//         }
//         playerCount++; 
//         UpdatePlayerCountUI(); 
//     }

//     public void PlayerLeft(PlayerRef player)
//     {
//         playerCount--;
//         UpdatePlayerCountUI();
//     }

//     private void UpdatePlayerCountUI()
//     {
//         if (playerCountText != null)
//         {
//             playerCountText.text = "/ " + playerCount.ToString();
//         }
//         else
//         {
//             Debug.LogError("PlayerNamesText is not assigned in the inspector");
//         }
//     }
  
// }

using Fusion;
using UnityEngine;
using TMPro;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    public TMP_Text playerCountText; // Link this in the Inspector

    private int playerCount = 0;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            var fusionBootstrap = FindObjectOfType<FusionBootstrap>();
            string uniqueName = fusionBootstrap.PlayerName;
            string avatarURL = fusionBootstrap.AvatarURL;

            Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity, player, (runner, spawnedPlayer) =>
            {
                Player playerScript = spawnedPlayer.GetComponent<Player>();
                playerScript.playerName = uniqueName;
                playerScript.SetAvatarURL(avatarURL);

                Debug.Log($"Player {uniqueName} joined with avatar URL: {avatarURL}");
            });
        }
        playerCount++;
        UpdatePlayerCountUI();
    }

    public void PlayerLeft(PlayerRef player)
    {
        playerCount--;
        UpdatePlayerCountUI();
    }

    private void UpdatePlayerCountUI()
    {
        if (playerCountText != null)
        {
            playerCountText.text = "/ " + playerCount.ToString();
        }
        else
        {
            Debug.LogError("PlayerNamesText is not assigned in the inspector");
        }
    }
}