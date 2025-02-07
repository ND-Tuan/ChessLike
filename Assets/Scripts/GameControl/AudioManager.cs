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
        Observer.AddListener(EvenID.BossMusic, OnPlayBossMusic);
        Observer.AddListener(EvenID.PlayFxSound, PlayFxSound);
    }

    void Start()
    {   
        FxSoundSources = FxSourcesRoot.GetComponentsInChildren<AudioSource>();
        
        MusicSource.clip = OffBattleMusic;
        MusicSource.Play();
    }

    private void OnBeginCombat(object[] data)
    {
        // MusicSource.clip = BattleMusic;
        // MusicSource.Play();
    }

    private void OnEndCombat(object[] data)
    {
        MusicSource.clip = OffBattleMusic;
        MusicSource.Play();
    }

    private void OnPlayBossMusic(object[] data)
    {
        AudioClip BossMusic = (AudioClip)data[0];
        MusicSource.clip = BossMusic;
        MusicSource.Play();
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
