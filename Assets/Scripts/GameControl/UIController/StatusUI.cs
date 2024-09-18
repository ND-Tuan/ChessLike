using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ObserverPattern;
using System.Threading.Tasks;

public class StatusUI : MonoBehaviour
{
    [Header("Player UI")]
    [SerializeField] private Slider _PlayerHpBar;
    [SerializeField] private TextMeshProUGUI _PlayCurrentHpText;
    [SerializeField] private TextMeshProUGUI _PlayerAmor;
    

    [Header("Status UI")]
    [SerializeField] private GameObject _ReloadBar;
    [SerializeField] private Slider _ReloadProgress;
    [SerializeField] private TextMeshProUGUI _PlayerCoinText;
    [SerializeField] private TextMeshProUGUI _PlayerAmmoText;
    [SerializeField] private Image _CurrentGunIcon;

    private int _ammo;

    void Awake()
    {
         //Đăng ký Event

            //Player
        Observer.AddListener(EvenID.DisplayPlayerHP, OnDisplayPlayerHp);
        Observer.AddListener(EvenID.DisplayReloadProgress, OnDisplayReloadProgress);

            //Enemy
        Observer.AddListener(EvenID.DisplayDamagePopup, OnDisplayDamagePopup);

            //Status
        Observer.AddListener(EvenID.DisplayCoin, OnDisplayCoin);
        Observer.AddListener(EvenID.DisplayCurrentGunIcon, OnDisplayCurrentGunIcon);
        Observer.AddListener(EvenID.DisplayPlayerAmmo, OnDisplayPlayerAmmo);
    }

    void Start()
    {

        _PlayerCoinText.text = CoinAndAmmoManager.GetPlayerCoin().ToString();
    }

    private void OnDisplayPlayerHp(object[] data)
    {
        int hp = (int)data[0];
        int maxHp = (int)data[1];

        _PlayerHpBar.maxValue = maxHp;
        _PlayerHpBar.value = hp;

        _PlayCurrentHpText.text = hp.ToString();
    }

    private async void OnDisplayReloadProgress(object[] data)
    {
        float reloadTime = (float)data[0];
        int ammo = (int)data[1];

        float elapsed = 0f;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _ReloadBar.SetActive(true);

        while (elapsed < reloadTime)
        {
            
            elapsed += Time.deltaTime; 

            _ReloadBar.transform.position = player.transform.position;
            _ReloadProgress.value = Mathf.Lerp(0, 1, elapsed / reloadTime);
            await Task.Yield();

            
        }

        OnDisplayPlayerAmmo(new object[] { ammo });
        _ReloadBar.SetActive(false);

    }

    private void OnDisplayPlayerAmmo(object[] data)
    {
        if(data != null)  _ammo = (int)data[0];
       
        _PlayerAmmoText.text = _ammo + " |  " + CoinAndAmmoManager.GetAmmo();
    }

    private void OnDisplayCoin(object[] data)
    {
        _PlayerCoinText.text = CoinAndAmmoManager.GetPlayerCoin().ToString();

    }

    private void OnDisplayCurrentGunIcon(object[] data){
       _CurrentGunIcon.sprite = (Sprite)data[0];
        
    }


    private void OnDisplayDamagePopup(object[] data)
    {
        string text = data[0].ToString();
        Vector3 position = (Vector3)data[1];
       
        TextPopup(text, position, 0.25f, 0.3f, 1);
        
    }

    void TextPopup(string text, Vector3 position, float hight, float RandomRange, float scale){

        GameObject textPopup = ObjectPoolManager.Instance.GetObject("PopupText");
        textPopup.SetActive(true);

        textPopup.GetComponent<TextMeshProUGUI>().text = text;
        textPopup.transform.localScale = Vector3.one * scale;

        if(RandomRange == 0) return;
        textPopup.transform.position = position
                        + new Vector3(Random.Range(-RandomRange,RandomRange),hight,Random.Range(-RandomRange,RandomRange));
    }

    private int  AnimateCount(int start, int end, float progress)
    {
        int value = (int)Mathf.Lerp(start, end, progress);
        return value;
    }

    void OnDestroy()
    {
        Observer.RemoveListener(EvenID.DisplayPlayerHP, OnDisplayPlayerHp);
        Observer.RemoveListener(EvenID.DisplayDamagePopup, OnDisplayDamagePopup);
        Observer.RemoveListener(EvenID.DisplayCoin, OnDisplayCoin);
        Observer.RemoveListener(EvenID.DisplayCurrentGunIcon, OnDisplayCurrentGunIcon);
    }
}
