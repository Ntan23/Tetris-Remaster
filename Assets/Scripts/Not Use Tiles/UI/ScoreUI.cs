using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private GameManager gm;

    void Start() 
    {
        gm = GameManager.Instance;

        UpdateScoreText();
    }

    public void UpdateScoreText()
    {
        scoreText.text = "Score : " + gm.GetScore().ToString();
    }
}
