using System.Collections;
using UnityEngine;

public class LevelUpUI : MonoBehaviour
{
    public void ShowLeveUpUI() => StartCoroutine(FadeInFadeOut());
    
    IEnumerator FadeInFadeOut()
    {
        void UpdateAlpha(float alpha) => GetComponent<CanvasGroup>().alpha = alpha;

        GetComponent<RectTransform>().localPosition = new Vector3(0, -50, 0);
        GetComponent<CanvasGroup>().alpha = 1.0f;
        yield return new WaitForSeconds(0.1f);
        LeanTween.value(gameObject, UpdateAlpha, 1f, 0f, 0.5f);
        LeanTween.moveLocalY(gameObject, 150f, 1.0f);
    }
}
