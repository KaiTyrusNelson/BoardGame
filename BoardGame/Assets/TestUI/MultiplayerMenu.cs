using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FishNet;
/// <summary> This is a temporary class which is primarily used for testing purposes<summary>
public sealed class MultiplayerMenu : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button connectButton;

    private void Start()
    {
        hostButton.onClick.AddListener(
            ()=>
            {
                InstanceFinder.ServerManager.StartConnection();
            }
        );

        connectButton.onClick.AddListener(() => {InstanceFinder.ClientManager.StartConnection();});
    }
}
