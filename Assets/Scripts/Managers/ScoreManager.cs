using UnityEngine;


public class ScoreManager : MonoBehaviour
{

    private int score;
    public int Score { get { return score; } }

    private int scoreMultiplayer = 1;

    public int AddScore()
    {
        score += 1 * scoreMultiplayer;
        return score;
    }

    public int MinusScore()
    {
        score -= 1 * scoreMultiplayer;
        return score;
    }

    public void ResetScore()
    {
        score = 0;
    }
}
