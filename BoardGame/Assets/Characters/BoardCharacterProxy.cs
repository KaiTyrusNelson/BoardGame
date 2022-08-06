using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BoardCharacterProxy : MonoBehaviour, IPointerClickHandler
{
    /// <OnClick> When a board character is clicked, it will display the avaliable moves to the character
    public Character associatedCharacter;

    [SerializeField] public SPUM_Prefabs animator;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        BoardGenerator.Instance.DisplayMovableSquares(associatedCharacter);
    }

    public void Update()
    {
        if (BoardGenerator.Instance.currentSelection != null)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,0.5f);
        }else{
            GetComponent<BoxCollider2D>().enabled = true;
            GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,1f);
        }
    }

    public void PlayAnimation(AnimationE m)
    {
        switch(m)
        {
            case(AnimationE.Idle):
            {
                animator.PlayAnimation("idle");
                break;
            }
            case(AnimationE.Attack):
            {
                animator.PlayAnimation("attack");
                break;
            }
            case(AnimationE.Run):
            {
                animator.PlayAnimation("run");
                break;
            }
        }
    }

    public void Flip()
    {
        
    }
}


public enum AnimationE
{
    Idle = 1,
    Attack,
    Run,
}

public enum Dir{
    Right=1,
    Left
}

