
using UnityEngine;
using System.Collections.Generic;
using System;
/// <summary> The movement pattern dictats wheter a character can move to a different square or not
public abstract class MovementPattern{
    /// <CheckMove> uses the board and the requested move in order to see if the character can be moved from one location to the other. 
    public abstract List<Vector2Int> CheckMove(Character c);
    public int range = 0;
    public static MovementPattern GetPattern(MovePattern m)
    {
        switch (m)
        {
            case(MovePattern.TestMovementPattern):
            {
                Debug.Log("Assigning test movement pattern");
                return new TestMovementPattern();
            }
            case(MovePattern.Bishop):
            {
                Debug.Log("Assigning bishop movement pattern");
                return new Bishop();
            }
            case(MovePattern.Rook):
            {
                Debug.Log("Assigning rook movement pattern");
                return new Rook();
            }
            case(MovePattern.Queen):
            {
                Debug.Log("Assigning queen movement pattern");
                return new Queen();
            }
            case(MovePattern.King):
            {
                Debug.Log("Assigning king movement pattern");
                return new King();
            }
        }

        return new TestMovementPattern();
    }

}

public enum MovePattern
{
    TestMovementPattern = 1,
    Bishop,
    Rook,
    Queen,
    King
}


