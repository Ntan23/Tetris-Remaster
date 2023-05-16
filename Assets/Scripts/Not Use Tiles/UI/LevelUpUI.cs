using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpUI : MonoBehaviour
{
    public void ShowLeveUpUI() => StartCoroutine(FadeInFadeOut());
    
    IEnumerator FadeInFadeOut()
    {
        void UpdateAlpha(float alpha) => GetComponent<CanvasGroup>().alpha = alpha;
        
        LeanTween.value(gameObject, UpdateAlpha, 0f, 1f, 1.0f);
        yield return new WaitForSeconds(1.5f);
        LeanTween.value(gameObject, UpdateAlpha, 1f, 0f, 1.0f);
    }
}
