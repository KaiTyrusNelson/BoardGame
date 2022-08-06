using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using System.Collections.Generic;
public class Character : NetworkBehaviour
{
    /// <summary> These are the networked variables which are carried over to the player <summary>
    [SyncVar] string id;
    [SyncVar] public Player OwnerPlayer;
    MovementPattern p;

    /// <summary> This is for client purposes, and will allow us to display the character with ease <summary>
    /// <summary> Movement finder, finds appropriate squares for the card to move to
    [SerializeField] MovePattern movement;
    [SerializeField] AttackPatterns atk_pattern;
    [SerializeField] int movementRange;
    [SerializeField] int attackRange;
    [SerializeField] public SPUM_Prefabs proxy;
    /// <oldLocation> used exclusivley by the gameboard for mapsyncing
    public Vector2Int oldLocation;

    #region Stats
    [SerializeField] int attack = 1;
    [SerializeField] int max_hp = 1;
    public int hp{get; private set;}
    #endregion

    public void Awake(){
        hp = max_hp;
    }
    public List<Vector2Int> FindAvaliableSquares()
    {
        /// <check> We will return null if the character doesn't exist on board 
        if (!GameBoard.Instance.activeCharacters.ContainsKey(this))
            return null;
        /// <Movement> gets the associated squares relating to our characters movement
        MovementPattern pattern = MovementPattern.GetPattern(movement);
        pattern.range = movementRange;
        return pattern.CheckMove(this);
    }

    public List<Vector2Int> FindAvaliableAttacks()
    {
        /// <check> We will return null if the character doesn't exist on board 
        if (!GameBoard.Instance.activeCharacters.ContainsKey(this))
            return null;
        /// <Movement> gets the associated squares relating to our characters movement
        AttackPattern pattern = AttackPattern.GetPattern(atk_pattern);
        pattern.range = attackRange;
        return pattern.CheckMove(this);
    }

    public void Attack(Character c)
    {
        Debug.Log("An attack event has occured");
        c.hp -= this.attack;
    }

}