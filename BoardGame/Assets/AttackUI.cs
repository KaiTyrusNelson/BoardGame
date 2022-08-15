using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackUI : MonoBehaviour
{
    [SerializeField] List<Character> TestCharacters1;
    [SerializeField] List<Character> TestCharacters2;


    public void Test()
    {
        Debug.Log("Activated test");
        StartCoroutine(ActivateAttackAnimation(TestCharacters1, TestCharacters2, 2));
    }

    /// <AttackUI> Client object which displays the UI Attack
    [SerializeField] Transform Team1LeftProxy;
    [SerializeField] Transform Team1RightProxy;

    [SerializeField] Transform Team2LeftProxy;
    [SerializeField] Transform Team2RightProxy;

    [SerializeField] GameObject primaryPanel;

    /// <calcPositions>Determines the positions in which to place objects within the UI GRID
    List<Vector3> calcPositions(int num, int team)
    {
        Transform left;
        Transform right;
        if (team == 0)
        {
            left = Team1LeftProxy;
            right = Team1RightProxy;
        }else
        {
            left = Team2LeftProxy;
            right = Team2RightProxy;
        }

        float delta = (right.position.x-left.position.x)/((num));

        List<Vector3> res = new List<Vector3>();

        for (int i =0; i < num; i++)
        {
            res.Add( (right.position +left.position)/2 +new Vector3(delta * (i-num/2), 0, 0));
        }

        return res;
    }

    public IEnumerator ActivateAttackAnimation(List<Character> team1, List<Character> team2, int attackingTeam)
    {
        primaryPanel.SetActive(true);
        /// <Instantiates> each of the character proxies
        List<Vector3> locs = calcPositions(team1.Count ,0);
        List<GameObject> g = new();
        for(int i =team1.Count-1; i >=0 ; i--)
        {
            SPUM_Prefabs c = Instantiate(team1[i].proxy, locs[i], Quaternion.identity);
            c.transform.SetParent(primaryPanel.transform);
            g.Add(c.gameObject);
            c.transform.localScale = new Vector3(-75, 75, 0);
        }
        List<GameObject> g2 = new();
        locs = calcPositions(team2.Count ,1);
        for(int j = 0; j < team2.Count; j++)
        {
            SPUM_Prefabs c = Instantiate(team2[j].proxy, locs[j], Quaternion.identity);
            c.transform.SetParent(primaryPanel.transform);
            g2.Add(c.gameObject);
            c.transform.localScale = new Vector3(75, 75, 0);
        }

        


       

        Transform attackedPanel;
        List<GameObject> attackers;

        if (attackingTeam == 1)
        {
            Debug.Log("Attacking team 1");
            attackers = g;
            attackedPanel = Team2LeftProxy;
        }else{
            Debug.Log("Attacking team 2");
            attackers = g2;
            attackedPanel = Team1RightProxy;
        }

        foreach(GameObject go in attackers)
        {
            StartCoroutine(Attack(go.GetComponent<SPUM_Prefabs>(), attackedPanel.position));
        }

        yield return new WaitForSeconds(2.5f);
        primaryPanel.SetActive(false);
        for (int i = 0; i < g.Count; i++)
        {
            Destroy(g[i]);
        }

        for (int i = 0; i < g2.Count; i++)
        {
            Destroy(g2[i]);
        }
    }

    public IEnumerator Attack(SPUM_Prefabs attacker, Vector3 position)
    {
        yield return new WaitForSeconds(0.15f);

        Vector3 position_previous = attacker.transform.position;

        attacker.transform.LeanMove(position, 0.45f);

        yield return new WaitForSeconds(0.45f);

        attacker.PlayAnimation("attack");

        yield return new WaitForSeconds(0.60f);

        attacker.PlayAnimation("idle");

        attacker.transform.LeanMove(position_previous, 0.45f);

        yield return new WaitForSeconds(0.45f);
    }

    /// <Singleton>
    public static AttackUI Instance;
    public void Awake()
    {
        Instance = this;
        GetComponent<Canvas>().worldCamera = Camera.main;
        primaryPanel.SetActive(false);
    }
}
