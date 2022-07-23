using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;
using UnityEngine;
public class Character : NetworkBehaviour
{
    /// <summary> These are the networked variables which are carried over to the player <summary>
    [SyncVar] string id;
    [SyncVar] int x_pos;
    [SyncVar] int y_pos;
    /// <summary> This is for client purposes, and will allow us to display the character with ease <summary>
    [SerializeField] public GameObject AssociatedProxyObject;
}