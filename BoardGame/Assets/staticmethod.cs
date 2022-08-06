using UnityEngine;
using System;
public static class staticmethod
{
    public static Vector2Int copy(this Vector2Int copyObj)
    {
        Vector2Int copy = new(copyObj[0], copyObj[1]);
        return copy;
    }
}