using UnityEngine;
using UnityEngine.UI;

public class HoldPieceUI : MonoBehaviour
{
    [SerializeField] private Image nextTetrominoImage;
    [SerializeField] private Sprite[] tetrominoSprites;

    public void UpdateSprite(int index)
    {
        nextTetrominoImage.color = Color.white;
        Color temp = nextTetrominoImage.color;
        temp.a = 1f;
        nextTetrominoImage.sprite = tetrominoSprites[index];
    }
}
