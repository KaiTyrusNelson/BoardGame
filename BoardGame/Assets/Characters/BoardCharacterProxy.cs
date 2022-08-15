using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(SpriteRenderer))]
public class BoardCharacterProxy : MonoBehaviour, IPointerClickHandler
{
    /// <OnClick> When a board character is clicked, it will display the avaliable moves to the character
    Character _as;
    public Character associatedCharacter{get=>_as; set{

        _hp = Instantiate(_hp);
        _hp.transform.SetParent(Player.player._CombatUi.transform);

        _as = value;

        _hp._hp.maxValue = _as.max_hp;
        _hp._hp.value = _as.hp;

        _hp._mp.maxValue = _as.max_mp;
        _hp._mp.value = _as.mp;

        _as.hpBar = _hp;
    }}

    [SerializeField] public SPUM_Prefabs animator;
    [SerializeField] HpBar _hp;


    

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Debug.Log("BCP CLicked");
        if (!GameBoard.Instance.activeCharacters.ContainsKey(associatedCharacter))
            return;
        
        Vector2Int location = GameBoard.Instance.activeCharacters[associatedCharacter];
        Player.player._info.SetCharacters(GameBoard.Instance.grid[location[0],location[1]].addedCharacters);
    
        BoardGenerator.Instance.DisplayMovableSquares(associatedCharacter);
        BoardGenerator.Instance.currentSelection = location;
    }
    

    public void Update()
    {
        if (BoardGenerator.Instance.currentSelection[0] != -1 || BoardGenerator.Instance.currentSelection[1] != -1)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,0.5f);
        }else{
            GetComponent<BoxCollider2D>().enabled = true;
            GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,1f);
        }

        _hp.transform.position = this.transform.position + new Vector3(0,0.5f,0);
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

    public void Flip(Dir d)
    {
        if (d == Dir.Right)
        {
            transform.localScale = new Vector3(-Math.Abs(transform.localScale.x), transform.localScale.y,0);
        }else{
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y,0);
        }
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

