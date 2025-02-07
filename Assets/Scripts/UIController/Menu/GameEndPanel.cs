using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEndPanel : MonoBehaviour
{
    [SerializeField] private Image _GunIcon;
    [SerializeField] private TextMeshProUGUI _timePlay;
    [SerializeField] private TextMeshProUGUI _RecievedBuffs;

    public void SetPanel(float timePlay, int buff, Sprite gunIcon, bool isWin){

        int minutes = Mathf.FloorToInt(timePlay / 60f);
        int seconds = Mathf.RoundToInt(timePlay % 60f);

        if (seconds == 60){
            seconds = 0;
            minutes += 1;
        }
        string Str = minutes.ToString("00") + "m" + seconds.ToString("00") + "s";

        if (minutes == 0) Str = seconds.ToString("00") + "s";
        
        _timePlay.text = Str;
        _RecievedBuffs.text = buff.ToString();
        _GunIcon.sprite = gunIcon;


    }


}
