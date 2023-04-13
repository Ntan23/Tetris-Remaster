using UnityEngine;
using UnityEngine.UI;

public class NextPieceUI : MonoBehaviour
{
    [SerializeField] private Image nextTetrominoImage;
    [SerializeField] private Sprite[] tetrominoSprites;

    public void UpdateSprite(int index) => nextTetrominoImage.sprite = tetrominoSprites[index];
}
