using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
    [SerializeField] private int _BeginningCoin;
    [SerializeField] private int _MaxAmmo;
    [SerializeField] private GameObject Chest;
    public int EnemyCounter { get; private set; }
    public float[] EnemyLevelUpScale;
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

        //Khởi tạo dữ liệu
        CoinAndAmmoManager.AddCoins(_BeginningCoin);
        CoinAndAmmoManager.SetMaxAmmo(_MaxAmmo);
        CoinAndAmmoManager.AddAmmo(_MaxAmmo);
    }

    void Start()
    {
        //Đăng ký Event
        Observer.AddListener(EvenID.BoardDone, OnBoardDone);

    }

 
    public void OnBoardDone(object[] data){
       _CurrentProsses++;
       Chest.SetActive(true);
       Chest.transform.position = (Vector3)data[0];
       BoardDone = false;
    }

    
}
