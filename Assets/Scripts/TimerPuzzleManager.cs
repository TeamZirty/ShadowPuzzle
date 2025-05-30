using UnityEngine;

public class TimerPuzzleManager : MonoBehaviour
{
    public int requiredCount;
    private int currentCount;
    private float timer;
    public float timeLimit = 5;

    private bool puzzleActive;

    private void Update()
    {
        if (puzzleActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                ResetPuzzle();
            }
        }
    }

    public void ActivatePuzzle()
    {
        if (!puzzleActive)
        {
            puzzleActive = true;
            timer = timeLimit;
            currentCount = 0;
        }
    }

    public void RegisterSuccess()
    {
        currentCount++;
        if (currentCount >= requiredCount)
        {
            Debug.Log("ÆÛÁñ ¼º°ø!");
            puzzleActive = false;
        }
    }

    public void ResetPuzzle()
    {
        Debug.Log("ÆÛÁñ ½ÇÆÐ, ¸®¼Â!");
        puzzleActive = false;
        currentCount = 0;
    }
}
