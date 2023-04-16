using UnityEngine;
using TMPro;

public class HighestUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highscoreText;
    [SerializeField] private TextMeshProUGUI bestLineClearedText;
    private int highscore;
    private int bestLienCleared;

    void Start()
    {
        highscore = PlayerPrefs.GetInt("Highscore", 0);
        bestLienCleared = PlayerPrefs.GetInt("BestLineCleared", 0);

        UpdateUI();
    }

    private void UpdateUI()
    {
        highscoreText.text = "Highscore : " + highscore.ToString();
        bestLineClearedText.text = "Best Line Cleared : " + bestLienCleared.ToString();
    }
}
