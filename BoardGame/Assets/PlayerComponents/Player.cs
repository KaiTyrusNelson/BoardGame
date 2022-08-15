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
    [SyncObject] public readonly SyncList<Vector2Int> placeableSquares = new SyncList<Vector2Int>();

    #endregion

    #region Server
    /// <summary> This will be used to submit commands over the network to the server<summary>
    [SerializeField] Character c;
    public override void OnStartServer()
    {
        base.OnStartServer();
        GamePlayerManager.AuthPlayer(this);
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

    #region PlayerInput
    public int lastCommand;
    public Vector2Int Location;
    public Vector2Int Location2;
    public Character selectedCharacter;

    [ServerRpc]
    public void InputSummon(Vector2Int loc, Character c)
    {
        lastCommand = (int)PlayerCommands.PlaceCharacter;
        Location = new Vector2Int(loc[0], loc[1]);
        selectedCharacter = c;
    }

    [ServerRpc]
    public void InputMovement(Vector2Int locationFrom, Vector2Int locationTo)
    {
        lastCommand = (int)PlayerCommands.MoveCharacter;
        Location = locationFrom;
        Location2 = locationTo;
    }

    [ServerRpc]
    public void InputAttack(Vector2Int locationFrom, Vector2Int locationTo)
    {
        lastCommand = (int)PlayerCommands.AttackCharacter;
        Location = locationFrom;
        Location2 = locationTo;
    }



    public void ClearInput(){
        lastCommand = (int)PlayerCommands.None;
    }
    #endregion

    #region Client
    /// <summary> Clientside functionality of the player object<summary>
    #region Attributes

    public static Player player;
    [SerializeField] public HandUI _handUI;
    [SerializeField] public BoardGenerator _board;
    [SerializeField] public AttackUI _attackUi;
    [SerializeField] public InfoUI _info;
    [SerializeField] public Canvas _CombatUi;
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
        _info = Instantiate(_info);
        _handUI.InitializeSelf();
        /// <summary> Instantiate to board UI <summary>
        _board = Instantiate(_board);
        _attackUi = Instantiate(_attackUi);
        _CombatUi = Instantiate(_CombatUi);
        /// <ResponseLiseners> Adds listeners which respond to changes in the syncList
        currentHand.OnChange +=  currentHand_OnChange;
        /// <UpdateBoard> if the player has connected late, we need to make sure their current board is up to date with what we see
    }

    #region ClientRpcs

    private void currentHand_OnChange(SyncListOperation op, int index,
    Character oldItem, Character newItem, bool asServer)
    {
        /// <summary> Function called by the server for the client to redraw its current hand <summmary>
        
        /// <call> calls to the handUI to redraw, we will make this function better later
        _handUI.UpdateHand();
    }

    [TargetRpc]
    public void CallAttack(NetworkConnection c, List<Character> lc1, List<Character> lc2, int num)
    {
        AnimationManager.AddAnimation(_attackUi.ActivateAttackAnimation(lc1, lc2, 1));
    }

    #endregion
    

    #endregion

}

public enum PlayerCommands
{
    None = 0,
    PlaceCharacter,
    MoveCharacter,
    AttackCharacter
}