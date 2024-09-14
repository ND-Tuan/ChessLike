using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
    public int EnemyCounter { get; private set; }
    public int _NumBoardBeforeBoss = 3;
    public int _CurrentProsses = 1;
    public List<EnemyWaveSetting> _enemyWaveSetting;
    
    public bool BoardDone;

    public static GameManager Instance { get; private set; }
    

    private void Awake()
    {
        //triển khai Singleton
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);

        } else if (Instance != this){
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //Đăng ký Event
        Observer.AddListener(EvenID.BoardDone, OnBoardDone);

    }

 
    public void OnBoardDone(object[] data){
       _CurrentProsses++;
       BoardDone = false;
    }

    
}
