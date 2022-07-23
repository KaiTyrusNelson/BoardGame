using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    /// <summary> This is the HandUI objeect for the client, this will display a set of characters which we can play <summary>
    
    /// <summary> This will be a dictionary binding the proxies to their original characters
    public Dictionary<GameObject, Character> proxies = new();
    /// <summary> We will keep track of the owner of the hand aswell <summary>
    public Player Owner;
    /// <summary> This is the panel which we will be binding our proxies to, which we <summary>
    [SerializeField] public Transform proxyPanel;

    public void InitializeSelf(Player p)
    {   
        Owner = p;
        foreach (Character c in p.currentHand)
        {
            GameObject go = Instantiate(c.AssociatedProxyObject);
            go.transform.SetParent(proxyPanel);
            proxies[go] = c; 
        }
    }
}
