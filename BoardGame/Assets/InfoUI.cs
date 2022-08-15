using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour
{
    [SerializeField] public Transform proxyPanel;

    public void Start()
    {
        proxyPanel.parent.GetComponent<Canvas>().worldCamera = Camera.main;
        proxyPanel.parent.GetComponent<Canvas>().sortingLayerID = SortingLayer.layers[2].id;
    }
    
    [SerializeField] Transform LeftProxy;
    [SerializeField] Transform RightProxy;

    [SerializeField] Slider hpSlider;
    [SerializeField] Slider mpSlider;

    [SerializeField] TMP_Text characterHpText;
    [SerializeField] TMP_Text characterMpText;

    /// <calcPositions>Determines the positions in which to place objects within the UI GRID
    List<Vector3> calcPositions(int num)
    {
        Transform left;
        Transform right;

        left = LeftProxy;
        right = RightProxy;

        float delta = (right.position.x-left.position.x)/((num));

        List<Vector3> res = new List<Vector3>();

        for (int i =0; i < num; i++)
        {
            res.Add( (right.position +left.position)/2 +new Vector3(delta * (i-num/2), 0, 0));
        }

        return res;
    }

    public List<Character> currentCharacters= new();
    public List<SPUM_Prefabs> currentProxies = new();
    int currentPosition=-1;

    public void Clear()
    {
        BoardGenerator.Instance.UndisplayMovableSquares();
        for(int i = 0; i < currentProxies.Count; i++)
        {
            Destroy(currentProxies[i].gameObject);
        }
        currentProxies = new();
    }

    public void Display()
    {
        List<Character> c = currentCharacters;
        
        if (currentPosition == -1)
        {
            List<Vector3> pos = calcPositions(c.Count);
            for (int i = 0; i < c.Count; i++)
            {
                SPUM_Prefabs n = Instantiate(c[i].proxy, pos[i], Quaternion.identity);
                n.transform.SetParent(proxyPanel.transform);
                n.transform.localScale = new Vector3(170,170,0);
                currentProxies.Add(n);
            }
        }else
        {
            Clear();
            List<Vector3> pos = calcPositions(1);
            SPUM_Prefabs n = Instantiate(c[currentPosition].proxy, pos[0], Quaternion.identity);
            n.transform.SetParent(proxyPanel.transform);
            n.transform.localScale = new Vector3(170,170,0);
            currentProxies.Add(n);

            Debug.Log($"{c[currentPosition].hp}/{c[currentPosition].max_hp}");
            Debug.Log($"{c[currentPosition].mp}/{c[currentPosition].max_mp}");

            hpSlider.maxValue = c[currentPosition].max_hp;
            mpSlider.maxValue = c[currentPosition].max_mp;

            hpSlider.value = c[currentPosition].hp;
            mpSlider.value = c[currentPosition].mp;

            characterHpText.text = $"{c[currentPosition].hp}/{c[currentPosition].max_hp}";
            characterMpText.text = $"{c[currentPosition].mp}/{c[currentPosition].max_mp}";

            // Display Moveable positions
            BoardGenerator.Instance.TheoreticalMoves = currentCharacters[currentPosition].FindAvaliableSquares(theory : true);
            BoardGenerator.Instance.TheoreticalAttacks = currentCharacters[currentPosition].FindAvaliableAttacks(theory : true);
        }
        
        currentPosition++;
        currentPosition = currentPosition % c.Count;
    }

    public void SetCharacters(List<Character> c)
    {
        Clear();
        currentPosition=-1;
        currentCharacters = c;
        Display();
    }


    #region 

    #endregion

}
