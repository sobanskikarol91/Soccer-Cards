using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [System.Serializable]
    public class PlayingFieldClip
    {
        public AudioClip _clip;
        public AudioMixerGroup output;
    }

    AudioSource refereeAS;
    public AudioSource crowdAS;

    [SerializeField]
    public PlayingFieldClip missClip;
    [SerializeField]
    public PlayingFieldClip goalClip;
    [SerializeField]
    public PlayingFieldClip cheering;
    [SerializeField]


    void Awake()
    {
        refereeAS = GetComponent<AudioSource>();

        if (instance == null) instance = this;
        else Destroy(this);
    }

    public void Goal()
    {
        crowdAS.clip = goalClip._clip;
        crowdAS.outputAudioMixerGroup = goalClip.output;
        crowdAS.Play();
    }


    public void Miss()
    {
        crowdAS.clip = missClip._clip;
        crowdAS.outputAudioMixerGroup = missClip.output;
        crowdAS.Play();
    }

    public void Whistle()
    {
        refereeAS.Play();
    }

    public void Cheering()
    {
        crowdAS.clip = cheering._clip;
        crowdAS.outputAudioMixerGroup = cheering.output;
        crowdAS.Play();
    }

    public void RandomClipSettings(AudioSource _as, float minPitch, float maxPitch)
    {
        float pitch = Random.Range(minPitch, maxPitch);
        float volume = Random.Range(_as.volume - _as.volume * 0.05f, _as.volume + _as.volume * 0.05f);

        _as.pitch = pitch;
        _as.volume = volume;

        _as.Play();
    }
}
