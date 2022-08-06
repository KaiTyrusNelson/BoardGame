using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class BoardGenerator : MonoBehaviour
{
    [SerializeField] List<Sprite> tiles;
    public static BoardGenerator Instance;
    /// <summary> this is the tile object that will be spawned <summary>
    [SerializeField] Tile tile;
    [SerializeField] GameObject static_tile;
    [SerializeField] BoardCharacterProxy proxyObj;
    Tile[,] associatedTransformGrid;
    public void Start()
    {
        var random = new System.Random();
        Instance = this;
        associatedTransformGrid = new Tile[GameBoard.Instance.Shape[0], GameBoard.Instance.Shape[1]];
        for (int i = -1; i < GameBoard.Instance.Shape[0]+1; i++)
        {
            for (int j = -6; j < GameBoard.Instance.Shape[1]+2; j++)
            {
                if (i >=0 && j >=0 && i<GameBoard.Instance.Shape[0] && j < GameBoard.Instance.Shape[1])
                {
                    
                    Tile t = Instantiate(tile, this.transform.position + new Vector3(i, j*.98f,0), Quaternion.identity);
                    if (Player.player == GamePlayerManager.Instance.Players[1])
                    {
                        t.x = i;
                        t.y = j;
                        associatedTransformGrid[i,j] = t;
                    }else{
                        t.x = GameBoard.Instance.Shape[0]-1-i;
                        t.y = GameBoard.Instance.Shape[1]-1-j;
                        associatedTransformGrid[t.x,t.y] = t;
                    }
                    t.GetComponent<SpriteRenderer>().sprite = tiles[random.Next(tiles.Count)];
                    t.GetComponent<SpriteRenderer> ().sortingOrder = -200 - j;
                    if (i%2==0 && j%2==0 || i%2==1 && j%2==1 )
                    {
                        t.defaultColor = new Color(0.95f, 1, 0.95f, 1);
                    }else{
                        t.defaultColor = new Color(0.78f, 0.8f, 0.78f, 1);
                    }
                    t.selectionColor = new Color(1, 0.92f, 0.016f, 1);
                    t.hoveredColor = new Color(1, 0, 0, 1);

                    t.transform.SetParent(this.transform);
                }else{
                    GameObject t = Instantiate(static_tile, this.transform.position + new Vector3(i, j*.98f,0), Quaternion.identity);
                    t.GetComponent<SpriteRenderer>().sprite = tiles[random.Next(tiles.Count)];
                    t.GetComponent<SpriteRenderer> ().sortingOrder = -200 - j;
                    t.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f, 1);
                    t.transform.SetParent(this.transform);
                }
            }
        }
    }


    /// <CharacterRenderation> We will use a dictionary to connect the character to its proxy object
    public Dictionary<Character, BoardCharacterProxy> characterProxies = new();
    public Character currentSelection;
    public List<Vector2Int> currentSelectionSquares;
    public List<Vector2Int> currentAttackSelectionSquares;

    /// <getLocationsForTilePlacement> given a size, it will place the characters on the appropriate tile
    public static List<Vector3> getLocationsForTilePlacement(int size, int x, int y)
    {
        // retrieve the width of a square
        float SquareWidth = Instance.tile.transform.localScale.x;
        // find the starting location
        if (size  == 1)
        {
            return new List<Vector3>(){Instance.associatedTransformGrid[x,y].transform.position};
        }

        List<Vector3> res = new();
        Vector3 start = Instance.associatedTransformGrid[x,y].transform.position - new Vector3(SquareWidth/4, 0, 0);

        res.Add(start);

        for (int i =1; i < size; i++)
        {
            Vector3 next = start + new Vector3(SquareWidth/ (2*(size-1)) * i, 0 ,0 ); 
            res.Add(next);
        }
        return res;
    }

    
    public static IEnumerator SummonCharacter(Character c, Vector2Int loc)
    {
        int x = loc[0];
        int y = loc[1];
        /// <AddressBoardProxies> first we must select all board proxies at the current location
        Debug.Log($"Trying to generate character {x} {y}");
        // Retrieve the character container
        CharacterContainer cc = GameBoard.Instance.grid[x, y];

        List<Vector3> locations = getLocationsForTilePlacement(cc.addedCharacters.Count, x , y);

        BoardCharacterProxy g = Instantiate(Instance.proxyObj, locations[locations.Count-1], Quaternion.identity);
        SPUM_Prefabs proxy = Instantiate(c.proxy, locations[locations.Count-1], Quaternion.identity);
        g.animator = proxy;
        proxy.transform.position -= new Vector3(0, 0.3f, 0);
        proxy.transform.localScale = new Vector3(0.9f,0.9f,0);
        proxy.transform.SetParent(g.transform);

        g.associatedCharacter = c;
        Instance.characterProxies.Add(c, g);

        for (int i =0; i < locations.Count && i < cc.addedCharacters.Count; i++)
        {
            if (Instance.characterProxies.ContainsKey(cc.addedCharacters[i]))
                Instance.characterProxies[cc.addedCharacters[i]].transform.LeanMove(locations[i], 0.5f).setEaseInOutQuart();
        }
        yield return new WaitForSeconds(0.5f);
    }

    public static IEnumerator MoveCharacter(Character c, Vector2Int loc)
    {

        Debug.Log("Running character");

        /// <checks> Makes sure the character is within the bounds
        if (!Instance.characterProxies.ContainsKey(c))
            yield break;

        CharacterContainer cc = GameBoard.Instance.grid[loc[0], loc[1]];

        List<Vector3> locations = getLocationsForTilePlacement(cc.addedCharacters.Count, loc[0] , loc[1]);
        float max_time = 0;
        for (int i =0; i < locations.Count && i < cc.addedCharacters.Count; i++)
        {
            if (Instance.characterProxies.ContainsKey(cc.addedCharacters[i]))
            {
                Vector3 currentPosition = Instance.characterProxies[cc.addedCharacters[i]].transform.position;
                max_time = Math.Max(0.5f * (locations[i] - currentPosition).magnitude, max_time);
                Instance.characterProxies[cc.addedCharacters[i]].transform.LeanMove(locations[i], 0.5f * (locations[i] - currentPosition).magnitude).setEaseInOutQuart();
                Instance.characterProxies[cc.addedCharacters[i]].PlayAnimation(AnimationE.Run);
            }
        }
        yield return new WaitForSeconds(max_time);

        for (int i =0; i < locations.Count && i < cc.addedCharacters.Count; i++)
        {
            if (Instance.characterProxies.ContainsKey(cc.addedCharacters[i]))
            {
                Instance.characterProxies[cc.addedCharacters[i]].PlayAnimation(AnimationE.Idle);
            }
        }
    }

    public static IEnumerator RemoveCharacter(Character c)
    {
        /// <checks> Makes sure the character is within the bounds
        if (!Instance.characterProxies.ContainsKey(c))
            yield break;

        Destroy(Instance.characterProxies[c].gameObject);
        Instance.characterProxies.Remove(c);

        yield return new WaitForSeconds(0.5f);
    }

    public void DisplayMovableSquares(Character c)
    {
        /// <Display> we will display all movable squares to the player currently avaliable
        if (!characterProxies.ContainsKey(c))
            return;
        /// <CurrentSelection> Tells the player what we are currently selecting
        currentSelection = c;
        /// <FindAvaliable> we will  then find the avaliable squares to move to
        List<Vector2Int> squares = c.FindAvaliableSquares();
        List<Vector2Int> attackSquares = c.FindAvaliableAttacks();
        currentSelectionSquares = squares;
        currentAttackSelectionSquares = attackSquares;
    }

    public void UndisplayMovableSquares()
    {
        currentSelection = null;
        List<Vector2Int> x = currentSelectionSquares;
        List<Vector2Int> y = currentAttackSelectionSquares;
        currentSelectionSquares = null;
        currentAttackSelectionSquares = null;

        if (x == null)
            return;
        foreach(Vector2Int selection in x)
        {
            SpriteRenderer r = associatedTransformGrid[selection[0], selection[1]].GetComponent<SpriteRenderer>();
            if (r!=null)
            {
               r.color = associatedTransformGrid[selection[0], selection[1]].defaultColor;
            }
        }
        foreach(Vector2Int selection in y)
        {
            SpriteRenderer r = associatedTransformGrid[selection[0], selection[1]].GetComponent<SpriteRenderer>();
            if (r!=null)
            {
               r.color = associatedTransformGrid[selection[0], selection[1]].defaultColor;
            }
        }
    }



}
