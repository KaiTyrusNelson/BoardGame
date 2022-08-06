using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public int x;
    public int y;

    /// <functionality> this region dictates the serer functionality of the tile
    public void OnDrop(PointerEventData data)
    {
        if (data.pointerDrag == null)
        {
            Debug.Log("No GameObject Found");
            return;
        }
        if (data.pointerDrag.GetComponent<Draggable>() == null)
        {
            Debug.Log("No Draggable Found");
            return;
        }
        Player.player.InputSummon(new Vector2Int(x,y), Player.player._handUI.proxies[data.pointerDrag]);
        Debug.Log($"Trying to place at {x} {y}");
    }

    /// <summary> Rendering <summary>

    
    Color _defaultColor;
    public Color selectionColor;
    public Color attackSelectionColor = new Color(1, 0, 1, 1);
    public Color defaultColor{
        get => _defaultColor;
        set
        {
            GetComponent<SpriteRenderer>().color = value;
            _defaultColor = value;
        }
    }
    public Color hoveredColor;

    void OnMouseOver()
    {
        GetComponent<SpriteRenderer>().color = hoveredColor;
    }

    void OnMouseExit()
    {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        GetComponent<SpriteRenderer>().color = defaultColor;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (BoardGenerator.Instance.currentSelection != null && BoardGenerator.Instance.currentSelectionSquares != null)
        {
            if (BoardGenerator.Instance.currentSelectionSquares.Exists( t => t[0] == x && t[1] == y))
            {
                Debug.Log($"Trying to move character to location {x} {y}");
                Player.player.InputMovement(GameBoard.Instance.activeCharacters[BoardGenerator.Instance.currentSelection] ,new Vector2Int(x,y));
            }
            if (BoardGenerator.Instance.currentAttackSelectionSquares.Exists( t => t[0] == x && t[1] == y))
            {
                Debug.Log($"Trying to move character to location {x} {y}");
                Player.player.InputAttack(GameBoard.Instance.activeCharacters[BoardGenerator.Instance.currentSelection] ,new Vector2Int(x,y));
            }
        }
        BoardGenerator.Instance.UndisplayMovableSquares();
    }

    public void Update()
    {
        /// <SelectionUpdate> If we are currently looking for selections, we will change the square color
        if (BoardGenerator.Instance.currentSelection != null && BoardGenerator.Instance.currentSelectionSquares != null)
        {
            if (BoardGenerator.Instance.currentSelectionSquares.Exists( t => t[0] == x && t[1] == y))
            {
                GetComponent<SpriteRenderer>().color = selectionColor;
            }
            if (BoardGenerator.Instance.currentAttackSelectionSquares.Exists( t => t[0] == x && t[1] == y))
            {
                GetComponent<SpriteRenderer>().color = attackSelectionColor;
            }
        }
    }

}
