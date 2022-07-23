using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    /// <summary> Populates our board object with transforms which we wil be able to place our objects on <summmary>
    [SerializeField] public Transform Panel;
    Transform[,] gridSquares = new Transform[7,7];

    public void Start()
    {
        int k = 0;
        for (int i =0; i < 7; i++)
        {
            for (int j=0; j < 7; j++)
            {
                gridSquares[i,j] = Panel.transform.GetChild(k).transform;
                DropSquareHandler d = Panel.transform.GetChild(k).GetComponent<DropSquareHandler>();
                d.x = i;
                d.y = j;
                k++;
            }
        }
    }
}
