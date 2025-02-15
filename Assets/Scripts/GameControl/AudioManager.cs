using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource MusicSource;
    [SerializeField] private AudioClip BattleMusic;
    [SerializeField] private AudioClip OffBattleMusic;
    [SerializeField] private Transform FxSourcesRoot;
	private AudioSource[] FxSoundSources;


    void Awake(){

        Observer.AddListener(EvenID.BeginCombat, OnBeginCombat);
        Observer.AddListener(EvenID.CombatDone, OnEndCombat);
        Observer.AddListener(EvenID.ChangeMusic, OnPlayMusic);
        Observer.AddListener(EvenID.PlayFxSound, PlayFxSound);
        Observer.AddListener(EvenID.StopMusic, StopMusic);
    }

    void Start()
    {   
        FxSoundSources = FxSourcesRoot.GetComponentsInChildren<AudioSource>();
        
        MusicSource.clip = OffBattleMusic;
        MusicSource.Play();
    }

    private void OnBeginCombat(object[] data)
    {
        if(MusicSource.clip == BattleMusic) return;
        MusicSource.clip = BattleMusic;
        MusicSource.Play();
    }

    private void OnEndCombat(object[] data)
    {
        if(MusicSource.clip == OffBattleMusic) return;
        MusicSource.clip = OffBattleMusic;
        MusicSource.Play();
    }

    private void OnPlayMusic(object[] data)
    {
        AudioClip Music = (AudioClip)data[0];
        MusicSource.clip = Music;
        MusicSource.Play();
    }

    private void StopMusic(object[] data)
    {
        MusicSource.Stop();
    }

    private void PlayFxSound(object[] data)
    {
        AudioClip clip = (AudioClip)data[0];
        Transform sourceTransform = (Transform)data[1];
        
        foreach (var source in FxSoundSources)
        {
            if (!source.isPlaying)
            {
                source.transform.position = sourceTransform.position;
                source.clip = clip;
                source.Play();
                return;
            }
        }
       
    }
    

    public void SetMusicVolume(float volume)
    {
        MusicSource.volume = volume;
    }

    public void SetFxVolume(float volume)
    {
        foreach (var source in FxSoundSources)
        {
            source.volume = volume;
        }
    }




}
