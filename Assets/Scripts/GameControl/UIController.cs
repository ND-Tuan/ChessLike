using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ObserverPattern;

public class UIController : MonoBehaviour
{
    [Header("Player UI")]
    [SerializeField] private Slider _PlayerHpBar;
    [SerializeField] private TextMeshProUGUI _PlayCurrentHpText;
    [SerializeField] private TextMeshProUGUI _PlayerAmor;

    void Start()
    {
        Observer.AddListener(EvenID.DisplayPlayerHP, OnDisplayPlayerHp);
        Observer.AddListener(EvenID.DisplayDamagePopup, OnDisplayDamagePopup);
    }

    void OnDisplayPlayerHp(object[] data)
    {
        int hp = (int)data[0];
        int maxHp = (int)data[1];

        _PlayerHpBar.maxValue = maxHp;
        _PlayerHpBar.value = hp;

        _PlayCurrentHpText.text = hp.ToString();
    }

    void OnDisplayDamagePopup(object[] data)
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
}
