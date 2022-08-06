/// <summary> The movement pattern dictats wheter a character can move to a different square or not
using System;
using System.Collections.Generic;
using UnityEngine;
public class TestMovementPattern : MovementPattern{
    /// <CheckMove> uses the board and the requested move in order to see if the character can be moved from one location to the other. 
    public override List<Vector2Int> CheckMove(Character c){
        List<Vector2Int> res = new();
        res.Add(new Vector2Int(0,0));
        res.Add(new Vector2Int(1,6));
        res.Add(new Vector2Int(3,3));
        return res;
    }
}