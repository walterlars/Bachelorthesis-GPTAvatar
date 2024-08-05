using UnityEngine;

public class InputHandlerUI : MonoBehaviour
{
    private AIManager aiManager;

    void Start()
    {
        GameObject gameLogic = GameObject.Find("GameLogic");
        if (gameLogic != null)
        {
            aiManager = gameLogic.GetComponent<AIManager>();
            if (aiManager == null)
            {
                Debug.LogError("AIManager component not found on GameLogic GameObject.");
            }
        }
        else
        {
            Debug.LogError("GameLogic GameObject not found.");
        }
    }

    void Update()
    {
        if (aiManager != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                aiManager.ToggleRecording();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                aiManager.RPC_OnAdviceButton();
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                aiManager.RPC_OnStopButton();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                aiManager.RPC_OnCopyButton();
            }
            if(Input.GetKeyDown(KeyCode.F))
            {
                aiManager.OnForgetButton();
            }
            //  if(Input.GetKeyDown(KeyCode.N))
            // {
            //     aiManager.OnNextFriend();
            // }
            //  if(Input.GetKeyDown(KeyCode.B))
            // {
            //     aiManager.OnPreviousFriend();
            // }
            // if(Input.GetKeyDown(KeyCode.P))
            // {
            //     aiManager.RPC_Evaluation();
            // }
        }
    }
}
