using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class NetworkButton : NetworkBehaviour
{
    public Button myButton;
    private Image buttonImage;
    private Color currentColor = Color.white;

    [Networked] private TickTimer _changeColorTimer { get; set; }

    private void Start()
    {
        buttonImage = myButton.GetComponent<Image>();
        myButton.onClick.AddListener(OnButtonPressed);
    }


    private void OnButtonPressed()
    {
        if (Runner.IsServer)
        {
            ChangeButtonColor();
            RPC_ChangeButtonColor(currentColor);
        }
        else
        {
            RPC_RequestColorChange();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_ChangeButtonColor(Color newColor)
    {
        buttonImage.color = newColor;
        currentColor = newColor;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestColorChange()
    {
        ChangeButtonColor();
        RPC_ChangeButtonColor(currentColor);
    }

    private void ChangeButtonColor()
    {
        currentColor = new Color(Random.value, Random.value, Random.value);
       
    }
}
