using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSFX : MonoBehaviour
{
    private AudioManager audioManager;

    void Start() => audioManager = AudioManager.Instance;

    public void PlayClickSFX() => audioManager.PlayButtonClickSFX();
}
