using System;
using System.Collections;
using System.Collections.Generic;
using ObserverPattern;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private GameObject _BlurBackground;
    [SerializeField] private GameObject _GunStoreUI;
    private int _OnSelect = 0;
    private int[] _ItemsCost = new int[3];
    [SerializeField] private TextMeshProUGUI _PlayerGold;
    [SerializeField] private TextMeshProUGUI _SelectedItemCost;
    [SerializeField] private GunInfoUI[] _GunInfoUI;

    // Start is called before the first frame update
    void Start()
    {
        //Đăng ký event
        Observer.AddListener(EvenID.DisplayGunStoreUI, OnDisplayGunStoreUI);
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

    private void OnDisplayGunStoreUI(object[] data){
       
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

    public void OnClose(){
        _BlurBackground.SetActive(false);
        _GunStoreUI.SetActive(false);
        Time.timeScale = 1;
    }

}
