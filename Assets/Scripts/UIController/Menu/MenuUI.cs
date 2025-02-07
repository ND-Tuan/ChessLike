using System;
using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [Serializable]
    private class GunInfoUI{
        public Image _Icon;
        public TextMeshProUGUI _Name;
        public TextMeshProUGUI _Dmg;
        public TextMeshProUGUI _CritDmg;
        public TextMeshProUGUI _CoolDown;
        public TextMeshProUGUI _AmmoCapacity;
        public TextMeshProUGUI _Cost;
    }

    [Serializable]
    private class BuffInfoUI{
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Description;
    }

    [SerializeField] private GameObject _BlurBackground;
    
    private int _OnSelect = 0;
    private int[] _ItemsCost = new int[3];

    [SerializeField] private GameObject _PauseUI;
    [SerializeField] private GameObject PauseBuffContainer;
    //cửa hàng súng
    [SerializeField] private GameObject _GunStoreUI;
    [SerializeField] private TextMeshProUGUI _PlayerGold;
    [SerializeField] private TextMeshProUGUI _SelectedItemCost;
    [SerializeField] private GunInfoUI[] _GunInfoUI;

    //Chọn buff
    [SerializeField] private GameObject _BuffSelectUI;
    [SerializeField] private BuffInfoUI[] _BuffInfoUI;

    //Cường hóa buff
    [SerializeField] private GameObject _BuffUpgradeUI;
    [SerializeField] private GameObject _BuffUpgradeContainer;
    [SerializeField] private TextMeshProUGUI _coin;
    [SerializeField] private TextMeshProUGUI _BuffUpgradeCost;
    [SerializeField] private TextMeshProUGUI _CurrentLevel;
    [SerializeField] private TextMeshProUGUI _NextLevel;
    [SerializeField] private TextMeshProUGUI _BuffName;
    [SerializeField] private List<BuffEffect> _PlayerBuffEffect ;
    private int _Cost;
    private List<GameObject> _BuffPanel = new List<GameObject>();


    //BossIntro
    [SerializeField] private GameObject _BossIntroUI;
    [SerializeField] private TextMeshProUGUI _BossName;
    [SerializeField] private Image _BossIcon;

    //GameComplete
    [SerializeField] private GameObject _GameOverUI;
    [SerializeField] private GameObject _GameWinUI;

    private bool IsAnyPanelActive = false;


    public static MenuUI Instance;

    // Start is called before the first frame update
    void Start()
    {   
        if (Instance == null){
            Instance = this;

        } else if (Instance != this){
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            if(_BlurBackground.activeInHierarchy){
                OnClose();
            } else {
                OnPause();
            }
        }
    }

    
    public void GetOnSelect(int select){
        _OnSelect = select-1;
        _SelectedItemCost.text = _ItemsCost[_OnSelect].ToString();

        if(!CoinAndAmmoManager.CanSpendCoins(_ItemsCost[_OnSelect])){
            _SelectedItemCost.color = Color.red;
        } else {
            _SelectedItemCost.color = Color.black;
        }
    }

    //======Pause=============================================
    public void OnPause(){
        _BlurBackground.SetActive(true);
        _PauseUI.SetActive(true);

        //Hiển thị buff
        ListingBuff(PauseBuffContainer);

        Time.timeScale = 0;
    }


    //=======Cưa hàng súng====================================
    public void OnDisplayGunStoreUI(object[] data){
       
        _BlurBackground.SetActive(true);
        _GunStoreUI.SetActive(true);
        Time.timeScale = 0;

        _PlayerGold.text = CoinAndAmmoManager.GetPlayerCoin().ToString();

        GunInfo _GunInfo ;

        //Hiển thị thông tin súng
        for(int i = 0; i < 3; i++){
            _GunInfo = (GunInfo)data[i];
            
            _GunInfoUI[i]._Icon.sprite = _GunInfo.Icon;
            _GunInfoUI[i]._Name.text = _GunInfo.Name;
            _GunInfoUI[i]._Dmg.text = _GunInfo.Damage.ToString();
            _GunInfoUI[i]._CritDmg.text = _GunInfo.CritDamage.ToString();
            _GunInfoUI[i]._CoolDown.text = (_GunInfo.Cooldown*100).ToString();
            _GunInfoUI[i]._AmmoCapacity.text = _GunInfo.AmmoCapacity.ToString();
            _GunInfoUI[i]._Cost.text = _GunInfo.Cost.ToString();

            _ItemsCost[i] = _GunInfo.Cost;
        }
    }


    public void OnBuy(){
        if(!CoinAndAmmoManager.CanSpendCoins(_ItemsCost[_OnSelect])) return;

        CoinAndAmmoManager.SpendCoins(_ItemsCost[_OnSelect]);
        Observer.PostEvent(EvenID.DropGun, _OnSelect);
       
        OnClose();
    }

    //===========End cửa hàng súng=============================


    //===========Chọn buff=====================================

    public void OnDisplayBuffSelectUI(object[] obj)
    {
        _BlurBackground.SetActive(true);
        _BuffSelectUI.SetActive(true);
        Time.timeScale = 0;

        BuffInfo[] _BuffInfo = (BuffInfo[])obj;

        int count = GameManager.Instance.GetBuffList().Count;
        if(count >=3) count = 3;
        
        for(int i = 0; i < count; i++){
            GameObject panel = _BuffInfoUI[i].Name.gameObject.transform.parent.gameObject;
            panel.SetActive(true);
            _BuffPanel.Add(panel);


            _BuffInfoUI[i].Name.text = _BuffInfo[i].BuffName;
            _BuffInfoUI[i].Description.text = _BuffInfo[i].description;
        }
    }

    public void OnSelectBuff(){
        Observer.PostEvent(EvenID.SelectBuff, _OnSelect);
        Debug.Log("Select buff: " + _OnSelect);
        OnClose();
    }

    //===========End chọn buff=================================

    //===========Cường hóa buff===============================
    public void OnDisplayBuffUpgradeUI(int cost){
        
        //Hiển thị UI
        _BlurBackground.SetActive(true);
        _BuffUpgradeUI.SetActive(true);
        Time.timeScale = 0;

        //lấy dữ liệu
        
        _Cost = cost;

        //hien thi thong tin
        _coin.text = CoinAndAmmoManager.GetPlayerCoin().ToString();
        _BuffUpgradeCost.text = (cost).ToString();
        
        if(!CoinAndAmmoManager.CanSpendCoins(_Cost)){
            _BuffUpgradeCost.color = Color.red;
        } else {
            _BuffUpgradeCost.color = Color.black;
        }

        //Hiển thị buff
        ListingBuff(_BuffUpgradeContainer);
        
        if(_OnSelect >= _PlayerBuffEffect.Count) return;
        DisplayUpgradeInfo(_OnSelect );
    }

    private void ListingBuff(GameObject container){
        _PlayerBuffEffect = GameManager.Instance.GetPlayerBuffList();
        for (int i = 0; i < _PlayerBuffEffect.Count; i++)
        {
            var buff = _PlayerBuffEffect[i];
            GameObject panel = ObjectPoolManager.Instance.GetObject("BuffPanel");
            if (panel == null) return;
        
            panel.SetActive(true);
            panel.transform.SetParent(container.transform);
            panel.GetComponent<BuffPanelDisplay>().DisplayBuff(buff.BuffName, buff.SetDescription(buff._isUpgraded ? 1 : 0), buff._isUpgraded);
            panel.GetComponent<BuffPanelDisplay>().SetBuffID(i);
        
            _BuffPanel.Add(panel);
        }
    }

    //Hiển thị thông tin cường hóa
    public void DisplayUpgradeInfo(int index){

        if(_BuffUpgradeUI.activeInHierarchy == false) return;
        _OnSelect = index;

        BuffEffect buff = _PlayerBuffEffect[_OnSelect];

        _BuffName.text = buff.BuffName;
        _CurrentLevel.text = buff.SetDescription(0);
        if(buff._isUpgraded) 
            _NextLevel.text = "Max level";
        else 
            _NextLevel.text = buff.SetDescription(1);
        
    }

    //Cường hóa buff
    public void OnUpgradeBuff(){
        if(_PlayerBuffEffect.Count == 0) return;
        if(!CoinAndAmmoManager.CanSpendCoins(_Cost)) return;
        if(_PlayerBuffEffect[_OnSelect]._isUpgraded) return;


        CoinAndAmmoManager.SpendCoins(_Cost);
        GameManager.Instance.UpgradeBuff(_OnSelect);
        
        //làm mới UI
        OnClose();
        OnDisplayBuffUpgradeUI(_Cost);

    }

    //===========End cường hóa buff===========================

    //===========BossIntro=====================================
    public void OnDisplayBossIntro(string name, Sprite icon){
        _BlurBackground.SetActive(true);
        _BossIntroUI.SetActive(true);
        Time.timeScale = 0;

        _BossName.text = name;
        _BossIcon.sprite = icon;
        Invoke(nameof(OnClose), 0.1f);
    }


    //===========GameOver=====================================
    public void OnDisplayGameOver(float timePlay, int buff, Sprite gunIcon){
        _BlurBackground.SetActive(true);
        _GameOverUI.SetActive(true);
        _GameOverUI.GetComponent<Animator>().Play("GameOverUI",-1, 0f);

        _GameOverUI.GetComponent<GameEndPanel>().SetPanel(timePlay, buff, gunIcon , false);
        
        Time.timeScale = 0;
    }

    //===========Game Complete================================
    public void OnDisplayGameWin(float timePlay, int buff, Sprite gunIcon){
        _BlurBackground.SetActive(true);
        _GameWinUI.SetActive(true);
        _GameWinUI.GetComponent<Animator>().Play("GameOverUI",-1, 0f);

        _GameWinUI.GetComponent<GameEndPanel>().SetPanel(timePlay, buff, gunIcon , false);

        Time.timeScale = 0;
    }

    public void Return(){
        
        SceneManager.LoadScene(0);
    }


    public void OnClose(){
        _BlurBackground.SetActive(false);
        _GunStoreUI.SetActive(false);
        _BuffSelectUI.SetActive(false);
        _BuffUpgradeUI.SetActive(false);
        _BossIntroUI.SetActive(false);
        _PauseUI.SetActive(false);

        foreach (var item in _BuffPanel)
        {
            item.SetActive(false);
        }

        _BuffPanel.Clear();

        Time.timeScale = 1;
    }

}
