using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour 
{   
    //Beginning Status
    [Header("---Beginning Status----------------------")]
    [SerializeField] private int _BeginningCoin;
    [SerializeField] private int _MaxAmmo;


    //Gameplay Status
    [Header("---Gameplay Status----------------------")]
    public float timePlay;
    public float[] EnemyLevelUpScale;
    public int _NumBoardBeforeBoss = 3;
    public int _CurrentStage = 1;
    public int _CurrentProgress = 1;
    public List<EnemyWaveSetting> _enemyWaveSetting;
    [SerializeField] private GameObject Chest;
    public bool BoardDone;
    
    //Buff manager
    [Header("---Buff Manager----------------------")]
    [SerializeField] private List<BuffEffect> _BuffList;
    [SerializeField] private List<BuffEffect> _PlayerBuffList = new();
    public bool _HasSoulEatingBuff { get; private set; }
    public int _HealAmount { get; private set; }
    
    [SerializeField] private int BurnDamage ;
    [SerializeField] private float BurnDuration ;
    [SerializeField] private float BurnCooldown ;

    [SerializeField] private bool _HasNoCostBuff;

    [SerializeField] private int PoisonDamage ;



    //Singleton
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

        _BuffList = Resources.LoadAll<BuffEffect>("Buff").ToList();


        //Đăng ký Event
        Observer.AddListener(EvenID.CombatDone, OnCombatDone);
        Observer.AddListener(EvenID.BoardDone, CalculateProgress);
        Observer.AddListener(EvenID.BuffSoulEating, SetSoulEatingBuff);
    }

    void Update()
    {
        timePlay += Time.deltaTime;
    }

    //Gameplay machenic=====================================
    public void OnCombatDone(object[] data){
        Chest.SetActive(true);
        Chest.transform.position = (Vector3)data[0];

        Observer.PostEvent(EvenID.BoardDone);
        BoardDone = false;
    }

    private void CalculateProgress(object[] data){
        _CurrentProgress++;
        if(_CurrentProgress > _NumBoardBeforeBoss+2){
            _CurrentProgress = 0;
            _CurrentStage++;
        }
    }




    //Buff machenic=====================================
    public List<BuffEffect> GetBuffList(){
        return _BuffList;
    }


    public List<BuffEffect> GetPlayerBuffList(){
        return _PlayerBuffList;
    }

    public void AddBuff(BuffEffect buff){
        _PlayerBuffList.Add(buff);
        _BuffList.Remove(buff);
        buff.ApplyBuff();
    }
    
    public void UpgradeBuff(int index){
        _PlayerBuffList[index].UpgradeBuff();
    }

    private void SetSoulEatingBuff(object[] obj){
        _HasSoulEatingBuff = true;
        _HealAmount = (int)obj[0];
    }

    public object[] GetBurnInfo(){
        return new object[]{BurnDamage, BurnDuration, BurnCooldown};
    }
}
