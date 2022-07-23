using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;
using FishNet.Connection;
public sealed class Player : NetworkBehaviour
{
    #region SyncVars
    /// <summary> This region displays the data which will stay consisted between the server and the client<summary>
    
    /// <currentHand> This syncs the characters currently in the players hand
    [SyncObject] public readonly SyncList<Character> currentHand= new SyncList<Character>();

    #endregion

    #region Server
    /// <summary> This will be used to submit commands over the network to the server<summary>
    [SerializeField] Character c;
    public override void OnStartServer()
    {
        base.OnStartServer();
        PlayerManager.ConnectPlayer(this);
        HandLoadManager.Instance.Aggregate(this);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        
    }

    public void AddCharacterToHand(Character c)
    {
        currentHand.Add(c);
    } 
    #endregion


    #region Client
    /// <summary> Clientside functionality of the player object<summary>

    #region Attributes

    public static Player player;
    [SerializeField] public HandUI _handUI;
    [SerializeField] public BoardUI _boardUI;
    #endregion
    public override void OnStartClient()
    {
        // ON THE CLIENT THIS WILL INSTANTIATE THE BOARD
        base.OnStartClient();
        
        if (!IsOwner)
            return;
        /// <summary> Instantiate the hand UI <summary>
        player = this;
        _handUI = Instantiate(_handUI);
        _handUI.InitializeSelf(this);
        /// <summary> Instantiate to board UI <summary>
        _boardUI = Instantiate(_boardUI);
    }

    

    #endregion

}