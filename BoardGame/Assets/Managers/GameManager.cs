using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;
public sealed class GameManager : NetworkBehaviour
{
    public static GameManager Instance {get; private set;}
    [SerializeField] GameObject debugTool;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        /// <summary> on the server we wish to instantiate the board <summary>
        if (IsServer)
        {
            Instantiate(debugTool);
        }
    }

}
