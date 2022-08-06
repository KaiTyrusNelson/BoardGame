using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
public class HandUI : MonoBehaviour
{
    /// <summary> This is the HandUI objeect for the client, this will display a set of characters which we can play <summary>
    
    /// <summary> This will be a dictionary binding the proxies to their original characters
    public Dictionary<GameObject, Character> proxies = new();
    /// <summary> We will keep track of the owner of the hand aswell <summary>
    [SerializeField] public Transform proxyPanel;
    [SerializeField] public Draggable charProxy;

    public void Start()
    {
        proxyPanel.parent.GetComponent<Canvas>().worldCamera = Camera.main;
    }
    public void InitializeSelf()
    {   
        foreach (Character c in Player.player.currentHand)
        {
            GameObject go = Instantiate(charProxy).gameObject;
            SPUM_Prefabs character = Instantiate(c.proxy);
            character.transform.SetParent(go.transform);
            character.transform.localScale = character.transform.localScale * (2f/3f);


            go.transform.SetParent(proxyPanel);
            proxies[go] = c; 
        }
    }


    public void UpdateHand()
    {
        /// <summary> updates the current players hand
        Debug.Log("Update hand RPC has been triggered");
        List<GameObject> keyList = new();
        foreach (GameObject g in proxies.Keys)
        {
            if (!Player.player.currentHand.Contains(proxies[g]))
            {
                keyList.Add(g);
            }
        }

        foreach (GameObject g in keyList)
        {
            proxies.Remove(g);
            Destroy(g);
        }
    }

    
    public void ToggleEnabled()
    {
        if (proxyPanel.gameObject.activeInHierarchy)
        {
            proxyPanel.gameObject.SetActive(false);
        }else
        {
            proxyPanel.gameObject.SetActive(true);
        }
    }
    public void SetEnabled(bool b)
    {
        proxyPanel.gameObject.SetActive(b);
    }

}
