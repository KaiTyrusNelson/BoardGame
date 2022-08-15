using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;
public sealed class GameManager : NetworkBehaviour
{
    public static GameManager Instance {get; private set;}
    [SerializeField] GameObject debugTool;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        /// <summary> on the server we wish to instantiate the board <summary>
        if (IsServer)
        {
            Instantiate(debugTool);
        }
    }


    [SyncVar]
    public Player currentPlayer;
    public IEnumerator GameLoop()
    {

        List<Vector2Int> player1Playable = GameBoard.Instance.QuerySquares((c) => (c[1] <= 1));
        List<Vector2Int> player2Playable = GameBoard.Instance.QuerySquares((c) => (c[1] >= 5));

        foreach (Vector2Int c in player1Playable)
            GamePlayerManager.Instance.Players[1].placeableSquares.Add(c);
        foreach (Vector2Int c in player2Playable)
            GamePlayerManager.Instance.Players[2].placeableSquares.Add(c);

        for (int i =0; i < 2; i++)
        {
            yield return StartCoroutine(awaitPlayerPlace(GamePlayerManager.Instance.Players[1]));
            yield return StartCoroutine(awaitPlayerPlace(GamePlayerManager.Instance.Players[2]));
        }



        for (int i =0; i < 100; i++)
        {
            yield return StartCoroutine(awaitMovePlayer(GamePlayerManager.Instance.Players[i%2+1]));
        }
        Debug.Log("GameLoopFinished");
        yield break;
    }

    public IEnumerator awaitPlayerPlace(Player player)
    {
        currentPlayer = player;
        Debug.Log("Awaiting player placement");
        while(true)
        {
            yield return null;
            if (player.lastCommand == (int)PlayerCommands.PlaceCharacter)
            {
                int x = player.Location[0];
                int y = player.Location[1];

                if (GameBoard.Instance.TryPlayToPosition(player, player.selectedCharacter, x, y))
                {
                    player.ClearInput();
                    Debug.Log("Playing character");
                    break;
                }else{
                    Debug.Log("Placement failed");
                    player.ClearInput();
                }
                    
            }
        }
    }

    public IEnumerator awaitMovePlayer(Player player)
    {
        currentPlayer = player;
        /// <TODO> CHECK IF ANY MOVE IS AVALIABLE
        Debug.Log("Awaiting player movement");
        while(true)
        {
            yield return null;
            if (player.lastCommand == (int)PlayerCommands.MoveCharacter)
            {
                if (GameBoard.Instance.TryMoveMultiple(player, player.Location, player.Location2))
                {
                    player.ClearInput();
                    break;
                }
                player.ClearInput();        
            }
            if (player.lastCommand == (int)PlayerCommands.AttackCharacter)
            {
                if (GameBoard.Instance.TryAttackMultiple(player, player.Location, player.Location2))
                {
                    Debug.Log("Ended");
                    player.ClearInput();
                    break;
                }
                player.ClearInput();        
            }
        }
        GameBoard.Instance.ClearCharacters();
    }
}
