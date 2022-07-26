using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    // FOR OUR DRAGGABLE WE WANT IT TO TRY AND ATTEMPT TO SEND A MESSAGE FOR A SET AMOUNT OF TIME AND THEN IF IT FAILS WE JUST WAIT
    
    public Transform returnTo;
    List<Vector2Int> returnTiles;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Picked up");
        returnTo = this.transform.parent;

        returnTo.gameObject.SetActive(false);
        this.transform.SetParent(this.transform.parent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts=false;

        BoardGenerator.Instance.UndisplayMovableSquares();

        returnTiles = new();
        foreach (Vector2Int l in Player.player.placeableSquares)
        {
            if (l!=null){
                BoardGenerator.Instance.associatedTransformGrid[l[0],l[1]].currentColor = new Color(1, 0.92f, 0.016f, 1);
                returnTiles.Add(new Vector2Int(l[0],l[1]));
            }
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane));
         this.transform.position = new Vector3(pos.x, pos.y, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Put down");
        //this.transform.position = new Vector3(returnTo.position.x, returnTo.position.y, 0);
        this.transform.SetParent(returnTo);
        returnTo.gameObject.SetActive(true);
        GetComponent<CanvasGroup>().blocksRaycasts=true;

        BoardGenerator.Instance.UndisplayMovableSquares();
    }

    void Update()
    {
        this.transform.localScale = new Vector3(1,1,0);
        foreach (Transform t in this.transform)
        {
                t.position = this.transform.position;
        }
    }
}
