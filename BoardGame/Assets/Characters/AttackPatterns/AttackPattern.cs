
using UnityEngine;
using System.Collections.Generic;
using System;
/// <AttackPattern> the attack pattern locates viable locations for the character to attack
public abstract class AttackPattern{
    /// <CheckMove> uses the board and the requested move in order to see if the character can be moved from one location to the other. 
    public abstract List<Vector2Int> CheckMove(Character c);
    public int range = 0;
    public static AttackPattern GetPattern(AttackPatterns m)
    {
        switch (m)
        {
            case(AttackPatterns.Bishop):
            {
                return new BishopAttack();
            }

        }

        return new BishopAttack();
    }
}

public enum AttackPatterns
{
    Bishop,
    Rook,
    Queen,
    Knight,
}

