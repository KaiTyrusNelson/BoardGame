using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;
using FishNet.Connection;
public sealed class HandLoadManager : NetworkBehaviour
{
    /// <summary> This class is entirely self contained in the server, and loads the players hand <summary>
    public static HandLoadManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    
    /// <summary> This will for now contains a list which we will aggregate directly to player on spawn <summmary>
    [SerializeField] public List<Character> aggregationList;
    public void Aggregate(Player p)
    {
        foreach (Character c in aggregationList)
        {
            /// <Spawning> Spawns the character into the scene
            GameObject spawnedCharacter = Instantiate(c.gameObject);
            InstanceFinder.ServerManager.Spawn(spawnedCharacter);

            /// <Assigment> assigns the neccesary properties to the created character
            Character C = spawnedCharacter.GetComponent<Character>();
            C.OwnerPlayer = p;
            p.AddCharacterToHand(C);
        }
    }
}