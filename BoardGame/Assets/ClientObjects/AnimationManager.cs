using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    static Queue<IEnumerator> runQ = new();

    public IEnumerator RunAnimation()
    {
        /// <Prechecks> waits until all the neccesary elements are initialized
        while (Player.player == null)
            yield return null;
        while (BoardGenerator.Instance == null)
            yield return null;
        Debug.Log("All animation prechecks passed");
        /// <RunAnimation> 
        while (true)
        {
            if  (runQ.Count > 0)
            {
                yield return StartCoroutine(runQ.Peek());
                runQ.Dequeue();
            }

            yield return null;
        }
    }

    public static void AddAnimation(IEnumerator NextAnim)
    {
        runQ.Enqueue(NextAnim);
    }

    public static AnimationManager Instance;

    public void Awake()
    {
        Instance = this;
        StartCoroutine(RunAnimation());
    }
}