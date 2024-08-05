// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using Fusion;
// using TMPro;

// public class PlayerNameManager : NetworkBehaviour
// {
//     // Shared list of player names
//     [Networked] public NetworkString<_256> playerNames { get; set; }

//      private int currentPlayerCount = 0;

//     public TMP_Text playerNameText;
//     public TMP_Text playerCountText;

//     public Button myButton;

//     private static List<string> names = new List<string>(); // Local list of player names

//     public void SetPlayerName(string name)
//     {
//         if (HasStateAuthority)
//         {
//             // Update the shared variable on the state authority
//             playerNames += name + ", ";
//             UpdateLocalPlayerNames();
//         }
//     }

//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     public void RpcUpdatePlayerNames(string name)
//     {
//         SetPlayerName(name);
//     }

//     public void ButtonPlayerName()
//     {
//         if (HasStateAuthority)
//         {
//             // If this instance has state authority, directly set the player name
//             SetPlayerName(playerNameText.text);
//             currentPlayerCount++;
//             UpdatePlayerCountUI();
//             HideButton();
//         }
//         else
//         {
//             // Otherwise, send an RPC to the state authority to update the player names
//             RpcUpdatePlayerNames(playerNameText.text);
//         }
//     }

//     private void UpdateLocalPlayerNames()
//     {
//         // Split the player names and update the local list
//         names = new List<string>(playerNames.ToString().Split(new string[] { ", " }, System.StringSplitOptions.RemoveEmptyEntries));
//     }
    
//     public override void FixedUpdateNetwork()
//     {
//         base.FixedUpdateNetwork();
//         if (Object.HasStateAuthority)
//         {
//             UpdateLocalPlayerNames();
//         }
//     }

    
//     void HideButton()
//     {
//         myButton.gameObject.SetActive(false);
//     }

//     private void UpdatePlayerCountUI()
//     {
//         if (playerCountText != null)
//         {
//             // playerCountText.text = "Players: " + playerCount.ToString();
//              playerCountText.text = currentPlayerCount.ToString();
//         }
//         else
//         {
//             Debug.LogError("PlayerNamesText is not assigned in the inspector");
//         }
//     }
// }


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class PlayerNameManager : NetworkBehaviour
{
    // Shared list of player names
    [Networked] public NetworkString<_256> playerNames { get; set; }

    [Networked] public int currentPlayerCount { get; set; }

    public TMP_Text playerNameText;
    public TMP_Text playerCountText;

    public Button myButton;

    private static List<string> names = new List<string>(); // Local list of player names

    public void SetPlayerName(string name)
    {
        if (HasStateAuthority)
        {
            // Update the shared variable on the state authority
            playerNames += name + ", ";
            UpdateLocalPlayerNames();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcUpdatePlayerNames(string name, PlayerRef player)
    {
        SetPlayerName(name);
        RpcUpdatePlayerCount(currentPlayerCount + 1);
        RpcHideButton(player);
    }

    public void ButtonPlayerName()
    {
        if (HasStateAuthority)
        {
            // If this instance has state authority, directly set the player name
            SetPlayerName(playerNameText.text);
            RpcUpdatePlayerCount(currentPlayerCount + 1);
            HideButton();
        }
        else
        {
            // Otherwise, send an RPC to the state authority to update the player names
            RpcUpdatePlayerNames(playerNameText.text, Runner.LocalPlayer);
        }
    }

    private void UpdateLocalPlayerNames()
    {
        // Split the player names and update the local list
        names = new List<string>(playerNames.ToString().Split(new string[] { ", " }, System.StringSplitOptions.RemoveEmptyEntries));
    }
    
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (Object.HasStateAuthority)
        {
            UpdateLocalPlayerNames();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcUpdatePlayerCount(int newCount)
    {
        currentPlayerCount = newCount;
        UpdatePlayerCountUI();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcHideButton(PlayerRef player)
    {
        if (Runner.LocalPlayer == player)
        {
            HideButton();
        }
    }

    void HideButton()
    {
        myButton.gameObject.SetActive(false);
    }

    private void UpdatePlayerCountUI()
    {
        if (playerCountText != null)
        {
            playerCountText.text = currentPlayerCount.ToString();
        }
        else
        {
            Debug.LogError("PlayerNamesText is not assigned in the inspector");
        }
    }
}
