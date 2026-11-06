using TMPro;
using UnityEngine;

public class ScoreDrawer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private TextMeshProUGUI highScoreText;

    private int highScore = 0;

    private void Start()
    {
        TheStack.OnScoreChanged += UpdateScore;
        highScore = TheStack.instance.GetHighScore();

        UpdateScore(0);
        DrawHighScore();
    }

    private void UpdateScore(int score)
    {
        scoreText.text = "Score: "+score.ToString();

        if (score > highScore)
        {
            highScore = score;
            DrawHighScore();
        }
    }

    private void DrawHighScore()
    {
        highScoreText.text = "High Score: " + highScore.ToString();
    }
}
