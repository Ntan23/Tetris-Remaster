using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager Instance {get; private set;}
    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.audioMixer;
        }
    }
    #endregion
    [SerializeField] private Sound[] sounds;

    private void Play(string name)
    {
        Sound s = System.Array.Find(sounds,sound=>sound.name==name);

        if(s == null) return;

        s.source.PlayOneShot(s.clip);
    }

    void Start() => Play("BGM");

    public void PlayBeepingSFX() => Play("Beeping");
}