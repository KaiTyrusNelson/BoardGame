using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropSquareHandler : MonoBehaviour, IDropHandler
{
    [SerializeField] public int x;
    [SerializeField] public int y;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject g = eventData.pointerDrag;

        Debug.Log($"Trying to place at {x} {y}");



        if (!Player.player._handUI.proxies.ContainsKey(g))
        {
            Debug.Log("Not an accetaple Drop Object");
            return;
        }

        Character chr = Player.player._handUI.proxies[g];
        GameBoard.Instance.TryPlayToPosition(chr, x, y);

        
    }
}
