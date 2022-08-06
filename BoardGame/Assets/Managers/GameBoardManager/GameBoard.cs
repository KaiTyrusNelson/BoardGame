using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
public class CharacterContainer
    {
        public List<Character> addedCharacters = new();
        public Player getOwnerPlayer()
        {
            if (addedCharacters.Count == 0)
                return null;
            return addedCharacters[0].OwnerPlayer;
        }
        public bool AddCharacter(Character c, int x, int y)
        {
            /// <check> If the container is not of the correct owner, EXIT
            if (c.OwnerPlayer != getOwnerPlayer() && !IsEmpty())
                return false;
            /// <AddCharacter> If it is, we will add the character to the collection
            if (!addedCharacters.Contains(c))
                addedCharacters.Add(c);
            c.oldLocation = new Vector2Int(x,y);
            Debug.Log($"{addedCharacters.Count}");
            return true;
        }
        public void RemoveCharacter(Character c)
        {
            addedCharacters.Remove(c);

        }
        public bool IsEmpty()
        {
            return (addedCharacters.Count == 0);
        }
    }

public class GameBoard : NetworkBehaviour
{
    #region SyncVars
    [SyncObject] public readonly SyncDictionary<Character, Vector2Int> activeCharacters = new();
    public static GameBoard Instance;

    /// <summary> The following stores information on the shape of the board<summary>
    [SerializeField] public int size_x;
    [SerializeField] public int size_y;
    public Vector2Int Shape {get => new Vector2Int(size_x, size_y);}
    #endregion

    #region Mapping

    public CharacterContainer[,] grid;

    private void board_OnChange(SyncDictionaryOperation op,
    Character character, Vector2Int value, bool asServer)
    {
        /* Key will be provided for
        * Add, Remove, and Set. */     
        int x = value[0];
        int y = value[1];
        switch (op)
        {
            // In the case that a new character is added, we will need to RENDER the new GameObject
            case SyncDictionaryOperation.Add:
                {
                    Debug.Log("Going to add character");
                    /// <check> If the grid space is avaliable for the current player
                    bool tryAdd = grid[x,y].AddCharacter(character, x, y);
                    /// <check> If it fails to add
                    if (!tryAdd)
                    {
                        /// <remove> remove the character
                        Debug.Log("Failed to add character");
                        activeCharacters.Remove(character);
                    }else{
                        Debug.Log("Added character succesfully");

                        if (!IsServer)
                        {
                            AnimationManager.AddAnimation(BoardGenerator.SummonCharacter(character, value));
                        }
                    }
                    break;
                }                
            //Removes key.
            case SyncDictionaryOperation.Remove:
                {
                    Debug.Log("Removing character");
                    /// <remove> remove the character
                    grid[character.oldLocation[0],character.oldLocation[1]].RemoveCharacter(character);

                    if (!IsServer)
                    {
                        AnimationManager.AddAnimation(BoardGenerator.RemoveCharacter(character));
                    }
                    break;                   
                }
            //Sets key to a new value.
            case SyncDictionaryOperation.Set:
                {
                    /// <removal> removes the character from its previous location
                    Debug.Log("Trying to move character");
                    grid[character.oldLocation[0], character.oldLocation[1]].RemoveCharacter(character);
                    bool tryAdd = grid[x,y].AddCharacter(character, x, y);
                    if (!tryAdd)
                    {
                        Debug.Log("Failed, trying agagin");
                        activeCharacters[character] = character.oldLocation.copy();
                    }else
                    {
                        Debug.Log("Move was successful");
                        if (!IsServer)
                            AnimationManager.AddAnimation(BoardGenerator.MoveCharacter(character, value));
                    }

                    break;
                }
                
            //Clears the dictionary.
            case SyncDictionaryOperation.Clear:
                break;
            //Like SyncList, indicates all operations are complete.
            case SyncDictionaryOperation.Complete:
                break;
        }
    }

    #endregion

    #region ServerExclusive
    [Server]
    public bool TryPlayToPosition(Player p, Character c, int x, int y)
    {
        Debug.Log($"Player has tried to play to locaiton {x} {y}");
        /// <checks> Confirms the player owns the current character
        if (!p.currentHand.Contains(c))
        {
            Debug.Log("Handcheck failed");
            return false;
        }
            
        /// <checks> we will add more checks later
        if (grid[x, y].getOwnerPlayer() != p && !grid[x,y].IsEmpty())
        {
            Debug.Log("Ownerplayer failed");
            return false;
        }
            
        /// <Removal> we will start by removing the played character from the current players hand
        p.currentHand.Remove(c);
        /// <Placement> we will then place the character onto the gameBoard
        activeCharacters.Add( c, new Vector2Int(x, y) );
        Debug.Log($"Player has tried to play to locaiton {x} {y}");

        return true;
    }
    
    [Server]
    public bool TryMove(Character c, Vector2Int location)
    {   
        Debug.Log("Received try move message");
        /// <checks> Confirms the character can be moved to said location
        if (!c.FindAvaliableSquares().Exists( x => x[0] == location[0] && x[1] == location[1]))
            return false;
        Debug.Log("Moving character");
        activeCharacters[c] = location;
        return true;
    }
    [Server]
    public bool TryAttack(Character c, Vector2Int location)
    {   
        Debug.Log("Received try attack message");
        /// <checks> Confirms the character can be moved to said location
        if (!c.FindAvaliableAttacks().Exists( x => x[0] == location[0] && x[1] == location[1]))
            return false;
        Debug.Log("Attacking character");
        
        foreach (Character _c in grid[location[0], location[1]].addedCharacters)
        {
            c.Attack(_c);
        }
        return true;
    }
    [Server]
    public bool TryMoveMultiple(Player player, Vector2Int location_from, Vector2Int location)
    {   
        if (!ConfirmOwner(location_from, player))
            return false;
        List<Character> chars = new List<Character>(grid[location_from[0], location_from[1]].addedCharacters);
        bool success = false;
        foreach (Character c2 in chars)
        {
            Debug.Log("Ran run");
            success = TryMove(c2, location)||success;
        }
        return true;
    }
    [Server]
    public bool TryAttackMultiple(Player player, Vector2Int location_from, Vector2Int location)
    {   
        if (!ConfirmOwner(location_from, player))
            return false;
        List<Character> chars = new List<Character>(grid[location_from[0], location_from[1]].addedCharacters);
        bool success = false;
        foreach (Character c2 in chars)
        {
            Debug.Log("Ran run");
            success = success || TryAttack(c2, location);
        }
        return success;
    }


    public void ClearCharacters()
    {
        List<Character> characters = new(activeCharacters.Keys);
        foreach (Character c in characters)
        {   if (c.hp <= 0)
                activeCharacters.Remove(c);
        } 
    }


    public bool ConfirmOwner(Vector2Int location_from, Player p)
    {
        return grid[location_from[0], location_from[1]].getOwnerPlayer() == p;
    }


    #endregion

    public void Awake()
    {
        Instance = this;
        grid = new CharacterContainer[size_x, size_y];

        for (int i =0; i < size_x; i++)
        {
            for (int j = 0; j < size_y; j++)
            {
                grid[i,j] = new();
            }
        }
        activeCharacters.OnChange += board_OnChange;
    }

    public void Update()
    {
        if (IsServer)
            return;
    }



}