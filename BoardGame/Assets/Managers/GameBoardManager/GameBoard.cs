using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class GameBoard : NetworkBehaviour
{
    // THE SYNC LIST OF CHARACTERS IS READABLE BY BOTH SERVER AND CLIENT
    [SyncObject] public readonly SyncList<Character> activeCharacters = new SyncList<Character>();


    public static GameBoard Instance;

    #region ServerExclusive
    /// <summary> This does not need to be networked and is stored exclusivley server-side for deicison<summary>
    public CharContainer[,] gameBoard = new CharContainer[7,7];
    #endregion

    
    public void Awake()
    {
        Instance = this;
    }


    [ServerRpc]
    public void TryPlayToPosition(Character c, int x, int y)
    {
        Debug.Log($"Player has tried to play to locaiton {x} {y}");
    }


}

public class CharContainer
{

}
