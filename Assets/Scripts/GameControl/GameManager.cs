using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.IO;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour 
{   
    [SerializeField]private PlayerController Player;

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
    public Vector3 CurrentBoardPosition = Vector3.zero;
    public List<EnemyWaveSetting> _enemyWaveSetting;
    [SerializeField] private GameObject Chest;
    public bool BoardDone;
    
    //Buff manager
    [Header("---Buff Manager----------------------")]
    [SerializeField] private List<BuffEffect> _BuffList;
    [SerializeField] private List<BuffEffect> _PlayerBuffList = new();
    public StatusEffectData _StatusEffectData;

    //Record
    

    //Singleton
    public static GameManager Instance { get; private set; }

    

    private void Awake()
    {
        //triển khai Singleton
        if (Instance == null){
            Instance = this;

        } else if (Instance != this){
            Destroy(gameObject);
        }

        //Khởi tạo dữ liệu
        CoinAndAmmoManager.AddCoins(_BeginningCoin);
        CoinAndAmmoManager.SetMaxAmmo(_MaxAmmo);
        CoinAndAmmoManager.AddAmmo(_MaxAmmo);

        //Load Buff
        _BuffList = Resources.LoadAll<BuffEffect>("Buff").ToList();

        //
        
      
        //Đăng ký Event
        Observer.AddListener(EvenID.CombatDone, OnCombatDone);
        Observer.AddListener(EvenID.BoardDone, CalculateProgress);
    }

    private List<float> GetRecordList(){
        string path = Application.dataPath + "/Record.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            RecordData data = JsonUtility.FromJson<RecordData>(json);
            return data.times;
        }
        else
        {
            return new List<float>();
        }
    }

    void Update()
    {
        timePlay += Time.deltaTime;
    }

    //Gameplay machenic=====================================
    public void GameOver(){
        Sprite gunIcon = Player.GetComponentInChildren<HolderController>().GetGunIcon();
        MenuUI.Instance.OnDisplayGameOver(timePlay, _PlayerBuffList.Count(), gunIcon);
    }

    public async void GameWin(){
        Player.CurrentState = PlayerController.PlayerState.Transforming;

        await Task.Delay(2000);
        Sprite gunIcon = Player.GetComponentInChildren<HolderController>().GetGunIcon();
        MenuUI.Instance.OnDisplayGameWin(timePlay, _PlayerBuffList.Count(), gunIcon);
        

        //Save data
        List<float> RecordList = GetRecordList();

        RecordList.Add(timePlay);
        RecordList = RecordList.OrderBy(t => t).ToList();

        // Giữ lại tối đa 8 kỷ lục
        if (RecordList.Count > 8)
        {
            RecordList = RecordList.GetRange(0, 8);
        }

        string json = JsonUtility.ToJson(new RecordData { times = RecordList });
        File.WriteAllText(Application.dataPath + "/Record.json", json);
    }

    public void OnCombatDone(object[] data){
        Chest.SetActive(true);
        Chest.transform.position = CurrentBoardPosition;

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

}


public class RecordData{
    public List<float> times;
}
