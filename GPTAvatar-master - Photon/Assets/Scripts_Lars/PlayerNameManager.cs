using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class PlayerNameManager : NetworkBehaviour
{
    [Networked] public NetworkString<_256> playerNames { get; set; }

    [Networked] public int currentPlayerCount { get; set; }

    public TMP_Text playerNameText;
    public TMP_Text playerCountText;

    public Button myButton;

    private static List<string> names = new List<string>();

    public void SetPlayerName(string name)
    {
        if (HasStateAuthority)
        {
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
            SetPlayerName(playerNameText.text);
            RpcUpdatePlayerCount(currentPlayerCount + 1);
            HideButton();
        }
        else
        {
            RpcUpdatePlayerNames(playerNameText.text, Runner.LocalPlayer);
        }
    }

    private void UpdateLocalPlayerNames()
    {
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
