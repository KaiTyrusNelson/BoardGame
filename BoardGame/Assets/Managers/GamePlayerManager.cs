using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;
public sealed class GamePlayerManager : NetworkBehaviour
{
    /// <summary> This is the player manager, and it is incharge of connecting and verifying players<summary>
    public static GamePlayerManager Instance {get; private set;}
    private void Awake()
    {
        Instance = this;
    }

    [SyncObject]
    public readonly SyncDictionary<int, Player> Players = new();
    /// <summary> For now we will just connect the players without any authentithication<summary>
    public static void AuthPlayer(Player p)
    {
        /// <summary> This will change later <summmary>
        if (!Instance.Players.ContainsKey(1))
        {
            Debug.Log("Player One has joined");
            Instance.Players.Add(1, p);
            HandLoadManager.Instance.Aggregate(p);
        }else if (!Instance.Players.ContainsKey(2)){
            Debug.Log("Player Two has joined");
            Instance.Players.Add(2,p);
            HandLoadManager.Instance.Aggregate(p);
        }
    }

    bool started = false;
    public void Update()
    {
        if (Players.Count == 2 && !started)
        {
            started = true;
            StartCoroutine(GameManager.Instance.GameLoop());
        }
    }
}

public static class PlayerExtensions
{
    public static Player OppositePlayer(this Player p)
    {
        if (p == GamePlayerManager.Instance.Players[1])
        {
            return GamePlayerManager.Instance.Players[2];
        }else
        {
            return GamePlayerManager.Instance.Players[1];
        }
    }
}