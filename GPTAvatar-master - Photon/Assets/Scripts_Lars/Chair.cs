using UnityEngine;
using Fusion;

public class Chair : NetworkBehaviour
{
    [Networked]
    public NetworkBool IsOccupied { get; set; }
}
