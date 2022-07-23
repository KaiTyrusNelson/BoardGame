using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;
public sealed class PlayerManager : NetworkBehaviour
{
    /// <summary> This is the player manager, and it is incharge of connecting and verifying players<summary>
    public static PlayerManager Instance {get; private set;}
    private void Awake()
    {
        Instance = this;
    }

    [SyncObject]
    public readonly SyncDictionary<int, Player> Players = new();
    /// <summary> For now we will just connect the players without any authentithication<summary>
    public static void ConnectPlayer(Player p)
    {
        /// <summary> This will change later <summmary>
        if (!Instance.Players.ContainsKey(1))
        {
            Debug.Log("Player One has joined");
            Instance.Players.Add(1, p);
            return;
        }else if (!Instance.Players.ContainsKey(2)){
            Debug.Log("Player Two has joined");
            Instance.Players.Add(2,p);
            return;
        }
    }
}
