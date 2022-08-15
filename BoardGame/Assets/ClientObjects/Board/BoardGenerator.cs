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
    public Tile[,] associatedTransformGrid;

    public void Start()
    {
        currentSelection = new Vector2Int(-1,-1);
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
                    t.hoveredColor = new Color(1,0.1f, 0.1f,1);
                    
                    t.currentColor = t.defaultColor;

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
    public Vector2Int currentSelection;

    List<Vector2Int> css;
    public List<Vector2Int> currentSelectionSquares{get=>css;
    set{
        if (value != null)
        {
            css = value;
            UpdateSquares(css, new Color(1, 0.92f, 0.016f, 1));
        }else{
            ReturnDefault();
            css = null;
        }
    }
    }

    List<Vector2Int> cass;
    public List<Vector2Int> currentAttackSelectionSquares{get=>cass;
    set{
        if (value != null)
        {
            cass = value;
            UpdateSquares(cass, new Color(1, 0, 0, 1));
        }else{
            ReturnDefault();
            cass = null;
        }
    }
    }

    /// <TheoreticalSquares>
    List <Vector2Int> tm;
    public List<Vector2Int> TheoreticalMoves{get=>tm;
    set{
        if (value != null)
        {
            tm = value;
            UpdateSquares(tm, new Color(0.5f, 0.5f, 1, 1));
        }else{
            ReturnDefault();
            tm = null;
        }
    }
    }

    List <Vector2Int> ta;
    public List<Vector2Int> TheoreticalAttacks{get=>ta;
    set{
        Debug.Log("Assigned theoretical attacks");
        if (value != null)
        {
            Debug.Log("Setting");
            ta = value;
            UpdateSquares(ta, new Color(1f, 0.5f, 0.5f, 0));
        }else{
            Debug.Log("Is null");
            ReturnDefault();
            ta = null;
        }
    }
    }

    public void UpdateSquares(List<Vector2Int> currentList, Color c)
    {
        if (currentList == null)
            return;
        foreach (Vector2Int inp in currentList)
        {
            associatedTransformGrid[inp[0], inp[1]].currentColor = c;
        }
    }

    public void ReturnDefault()
    {
        foreach (Tile t in associatedTransformGrid)
        {
            t.currentColor = t.defaultColor;
        }
    }

    
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
                if (currentPosition.x > locations[i].x)
                {
                    Instance.characterProxies[cc.addedCharacters[i]].Flip(Dir.Left);
                }else
                {
                    Instance.characterProxies[cc.addedCharacters[i]].Flip(Dir.Right);
                }
            }
        }
        yield return new WaitForSeconds(max_time);

        for (int i =0; i < locations.Count && i < cc.addedCharacters.Count; i++)
        {
            if (Instance.characterProxies.ContainsKey(cc.addedCharacters[i]))
            {
                Instance.characterProxies[cc.addedCharacters[i]].PlayAnimation(AnimationE.Idle);
                Instance.characterProxies[cc.addedCharacters[i]].Flip(Dir.Left);
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
        if (!GameBoard.Instance.activeCharacters.ContainsKey(c))
            return;
        /// <CurrentSelection> Tells the player what we are currently selecting
        Vector2Int loc = GameBoard.Instance.activeCharacters[c];
        /// <FindAvaliable> we will  then find the avaliable squares to move to
        currentSelectionSquares = null;
        currentAttackSelectionSquares = null;


        List<Vector2Int> res = new();
        List<Vector2Int> res2 = new();

        foreach (Character c2 in GameBoard.Instance.grid[loc[0], loc[1]].addedCharacters)
        {
            foreach(Vector2Int v2i in c2.FindAvaliableSquares())
            {
                res.Add(v2i);
            } 
            foreach(Vector2Int v2i in c2.FindAvaliableAttacks())
            {
                res2.Add(v2i);
            }
        }
        currentSelectionSquares = res;
        currentAttackSelectionSquares = res2;
    }

    public void UndisplayMovableSquares()
    {
        currentSelection = new Vector2Int(-1,-1);
        currentSelectionSquares = null;
        currentAttackSelectionSquares = null;
        TheoreticalMoves = null;
        TheoreticalAttacks = null;
    }



}
