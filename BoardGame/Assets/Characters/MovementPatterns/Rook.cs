using UnityEngine;
using System.Collections.Generic;
public class Rook : MovementPattern
{
    public override List<Vector2Int> CheckMove(Character c,bool theoretical = false){
        List<Vector2Int> res = new();
        Vector2Int location = GameBoard.Instance.activeCharacters[c];
        Recurse(range, location[0], location[1]+1, 0, 1, res , c, actual : !theoretical);
        Recurse(range, location[0]+1, location[1], 1, 0, res , c, actual : !theoretical);
        Recurse(range, location[0]-1, location[1], -1, 0, res , c, actual : !theoretical);
        Recurse(range, location[0], location[1]-1, 0, -1, res , c, actual : !theoretical);
        return res;
    }
    public void Recurse(int rangeRemaining, int x, int y, int plus_x, int plus_y, List<Vector2Int> res, Character c, bool actual = true)
    {
        if (x < 0  || y < 0 || x >= GameBoard.Instance.size_x || y>= GameBoard.Instance.size_y)
            return;
        if (GameBoard.Instance.grid[x, y].getOwnerPlayer() != c.OwnerPlayer && !GameBoard.Instance.grid[x, y].IsEmpty() && actual)
            return;
        if (rangeRemaining == 0)
            return;
        res.Add(new Vector2Int(x, y));
        Recurse(rangeRemaining-1, x + plus_x, y + plus_y, plus_x, plus_y, res , c, actual:actual);
    }
}