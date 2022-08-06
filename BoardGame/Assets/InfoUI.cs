using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoUI : MonoBehaviour
{
    [SerializeField] public Transform proxyPanel;
    [SerializeField] public GameObject charProxy;

    public void Start()
    {
        proxyPanel.parent.GetComponent<Canvas>().worldCamera = Camera.main;
        proxyPanel.parent.GetComponent<Canvas>().sortingLayerID = SortingLayer.layers[2].id;
    }
}
