using UnityEngine;
using TMPro;

public class LineClearedUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lineClearedText;
    private GameManager gm;

    void Start() 
    {
        gm = GameManager.Instance;

        UpdateLineClearedText();
    }

    public void UpdateLineClearedText() => lineClearedText.text = "Line Cleared: " + gm.GetLineCleared().ToString();
}
