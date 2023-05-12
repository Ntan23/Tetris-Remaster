using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    private GameManager gm;

    void Start() 
    {
        gm = GameManager.Instance;

        UpdateLevelText();
    }
        
    public void UpdateLevelText() => levelText.text = gm.GetLevelIndex().ToString();
}
