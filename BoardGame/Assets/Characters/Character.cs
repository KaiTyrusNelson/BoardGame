using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using System;
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
    [SyncVar] public int attack;
    [SyncVar(OnChange=nameof(onMaxHpChange))] public int max_hp;
    [SyncVar(OnChange=nameof(onMaxMpChange))] public int max_mp;
    [SyncVar(OnChange=nameof(onHpChange))] int hp_;
    [SyncVar(OnChange=nameof(onMpChange))] int mp_;

    public int hp{get => hp_; 
    set 
    {
        value = Math.Min (value, max_hp);
        value = Math.Max (value, 0);
        hp_ = value;
    }
    }
    
    public int mp{get => mp_; 
    set 
    {
        value = Math.Min (value, max_mp);
        value = Math.Max (value, 0);
        mp_ = value;
    }
    }
    #endregion



    public void Awake(){
        hp = max_hp;
        mp = 0;
    }
    public List<Vector2Int> FindAvaliableSquares(bool theory = false)
    {
        /// <check> We will return null if the character doesn't exist on board 
        if (!GameBoard.Instance.activeCharacters.ContainsKey(this))
            return null;
        /// <Movement> gets the associated squares relating to our characters movement
        MovementPattern pattern = MovementPattern.GetPattern(movement);
        pattern.range = movementRange;
        return pattern.CheckMove(this, theoretical : theory);
    }

    public List<Vector2Int> FindAvaliableAttacks(bool theory = false)
    {
        /// <check> We will return null if the character doesn't exist on board 
        if (!GameBoard.Instance.activeCharacters.ContainsKey(this))
            return null;
        /// <Movement> gets the associated squares relating to our characters movement
        AttackPattern pattern = AttackPattern.GetPattern(atk_pattern);
        pattern.range = attackRange;
        return pattern.CheckMove(this, theoretical : theory);
    }



    public void Attack(Character c)
    {
        Debug.Log("An attack event has occured");
        c.hp -= this.attack;
    }


    /// CLIENTSIDE FUNCTIONS

    public HpBar hpBar;

    void onHpChange(int o, int n, bool asServer)
    {
        Debug.Log($"{o}, {n}, {asServer}");
        if (hpBar != null)
        {
            Debug.Log($"Shifting value to {hp_}");
            hpBar._hp.maxValue = 1000;
            hpBar._hp.value = hp_;  
            
        }
    }

    void onMaxHpChange(int o, int n, bool asServer)
    {
        Debug.Log("MaxHp change logged");
        if (hpBar != null)
            hpBar._hp.maxValue = n;  
    }

    void onMpChange(int o, int n, bool asServer)
    {
        if (hpBar != null)
            hpBar._mp.value = n;     
    }

    void onMaxMpChange(int o, int n, bool asServer)
    {
        if (asServer)
        if (hpBar != null)
            hpBar._mp.maxValue = n;  
    }

}